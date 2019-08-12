using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class GasManager : MapComponent
    {
        public List<PipelineNet> PipeNets = new List<PipelineNet>();

        public List<CompPipe> cachedPipes = new List<CompPipe>();

        public int[,] PipeGrid;

        public bool[] DirtyPipe;

        public int masterID;

        public GasManager(Map map) : base(map)
        {
            int length = Enum.GetValues(typeof(PipeType)).Length;
            PipeGrid = new int[length, map.cellIndices.NumGridCells];
            DirtyPipe = new bool[length];
            for (int i = 0; i < DirtyPipe.Length; i++)
            {
                DirtyPipe[i] = true;
            }
        }

        public void RegisterPipe(CompPipe pipe, bool respawningAfterLoad)
        {
            if (!cachedPipes.Contains(pipe))
            {
                cachedPipes.Add(pipe);
                GenList.Shuffle(cachedPipes);
            }
            DirtyPipeGrid(pipe.PipeType);
            if (!respawningAfterLoad)
            {
                RegenGrids();
            }
        }

        public void DirtyPipeGrid(PipeType p)
        {
            DirtyPipe[(int)p] = true;
        }

        public void RegenGrids()
        {
            for (int i = 0; i < DirtyPipe.Length; i++)
            {
                if (DirtyPipe[i])
                {
                    RebuildPipeGrid(i);
                }
            }
        }

        public void RebuildPipeGrid(int P)
        {
            DirtyPipe[P] = false;
            for (int i = 0; i < PipeGrid.GetLength(1); i++)
            {
                PipeGrid[P, i] = -1;
            }
            PipeNets.RemoveAll((PipelineNet x) => x.NetType == P);
            (from x in cachedPipes
             where x.PipeType == (PipeType)P
             select x).ToList().ForEach(delegate (CompPipe j)
             {
                 j.GridID = -1;
             });
            for (CompPipe compPipe = cachedPipes.FirstOrDefault(delegate (CompPipe k)
            {
                if (k.PipeType == (PipeType)P)
                {
                    return k.GridID == -1;
                }
                return false;
            }); compPipe != null; compPipe = cachedPipes.FirstOrDefault(delegate (CompPipe k)
            {
                if (k.PipeType == (PipeType)P)
                {
                    return k.GridID == -1;
                }
                return false;
            }))
            {
                PipelineNet newNet = Activator.CreateInstance(typeof(PipelineNet)) as PipelineNet;
                newNet.GasManager = this;
                newNet.NetID = masterID;
                newNet.NetType = P;
                PipeNets.Add(newNet);
                Predicate<IntVec3> predicate = delegate (IntVec3 c)
                {
                    foreach (ThingWithComps item in GridsUtility.GetThingList(c, base.map).OfType<ThingWithComps>())
                    {
                        CompPipe compPipe2 = item.GetComps<CompPipe>().FirstOrDefault((CompPipe x) => x.PipeType == (PipeType)P);
                        if (compPipe2 != null && compPipe2.PipeType == (PipeType)P)
                        {
                            compPipe2.GridID = masterID;
                            compPipe2.pipeNet = newNet;
                            newNet.PipedThings.Add(compPipe2.parent);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, 4);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, 1);
                            PipeGrid[P, map.cellIndices.CellToIndex(c)] = masterID;
                            return true;
                        }
                    }
                    return false;
                };
                Action<IntVec3> action = delegate
                {
                };
                map.floodFiller.FloodFill(compPipe.parent.Position, predicate, action, int.MaxValue, false, (IEnumerable<IntVec3>)null);
                masterID++;
            }
            foreach (PipelineNet pipeNet in PipeNets)
            {
                pipeNet.InitNet();
            }
        }
    }
}
