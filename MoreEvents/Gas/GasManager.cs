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

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            RegenGrids();
        }

        public void DeregisterPipe(CompPipe pipe)
        {
            if (cachedPipes.Contains(pipe))
            {
                cachedPipes.Remove(pipe);
                GenList.Shuffle(cachedPipes);
            }
            DirtyPipeGrid(pipe.PipeType);
            RegenGrids();
        }

        public void RebuildPipeGrid(int P)
        {
            DirtyPipe[P] = false;
            for (int i = 0; i < PipeGrid.GetLength(1); i++)
            {
                PipeGrid[P, i] = -1;
            }

            PipeNets.RemoveAll((PipelineNet x) => (int)x.NetType == P);

            (from x in cachedPipes
             where x.PipeType == (PipeType)P
             select x).ToList().ForEach(delegate (CompPipe j)
             {
                 j.GridID = -1;
             });

            foreach(var pipe in cachedPipes)
            {
                if(pipe.PipeType == (PipeType)P)
                {
                    pipe.GridID = -1;
                }
            }

            CompPipe randomPipe = cachedPipes.Where(x => x.PipeType == (PipeType)P && x.GridID == -1).FirstOrDefault();

            for (CompPipe compPipe = cachedPipes.FirstOrDefault((CompPipe k) => k.PipeType == (PipeType)P && k.GridID == -1); compPipe != null; compPipe = cachedPipes.FirstOrDefault((CompPipe k) => k.PipeType == (PipeType)P && k.GridID == -1))
            {
                Log.Message("1");
                PipelineNet newNet = Activator.CreateInstance(typeof(PipelineNet)) as PipelineNet;
                newNet.GasManager = this;
                newNet.NetID = masterID;
                newNet.NetType = (PipeType)P;
                PipeNets.Add(newNet);
                Predicate<IntVec3> predicate = delegate (IntVec3 c)
                {
                    foreach (ThingWithComps item in GridsUtility.GetThingList(c, map).OfType<ThingWithComps>())
                    {
                        CompPipe compPipe2 = item.GetComp<CompPipe>();
                        if (compPipe2 != null && compPipe2.PipeType == (PipeType)P)
                        {
                            compPipe2.GridID = masterID;
                            compPipe2.pipeNet = newNet;
                            newNet.PipesThings.Add(compPipe2.parent);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, MapMeshFlag.Buildings);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, MapMeshFlag.Things);
                            PipeGrid[P, map.cellIndices.CellToIndex(c)] = masterID;
                            return true;
                        }
                    }
                    return false;
                };

                map.floodFiller.FloodFill(randomPipe.parent.Position, predicate, delegate (IntVec3 x) { });
                masterID++;
            }

            foreach (PipelineNet pipeNet in PipeNets)
            {
                pipeNet.InitNet();
            }
        }

        public bool ZoneAt(IntVec3 pos, PipeType P)
        {
            return PipeGrid[(int)P, map.cellIndices.CellToIndex(pos)] >= 0;
        }
    }
}
