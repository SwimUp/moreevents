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

        public int[] masterID;

        public GasManager(Map map) : base(map)
        {
            int length = Enum.GetValues(typeof(PipeType)).Length;
            PipeGrid = new int[length, map.cellIndices.NumGridCells];
            DirtyPipe = new bool[length];
            masterID = new int[length];

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
            }
            DirtyPipeGrid(pipe.PipeType);
            if (!respawningAfterLoad)
            {
                RegenGrids();
            }
        }

        public override void MapGenerated()
        {
            base.MapGenerated();
            RegenGrids();
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
            }
            DirtyPipeGrid(pipe.PipeType);
            RegenGrids();
        }

        public void DirtyAllPipeGrids()
        {
            for (int i = 0; i < DirtyPipe.Length; i++)
            {
                DirtyPipe[i] = true;
            }
            RegenGrids();
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            for(int i = 0; i < PipeNets.Count; i++)
            {
                PipeNets[i].PipelineNetTick();
            }
        }

        public void RebuildPipeGrid(int P)
        {
            DirtyPipe[P] = false;
            for (int i = 0; i < PipeGrid.GetLength(1); i++)
            {
                PipeGrid[P, i] = -1;
            }
            cachedPipes.Where(x => x.PipeType == (PipeType)P).ToList().ForEach(act => act.GridID = -1);

            PipeNets.RemoveAll((PipelineNet x) => (int)x.NetType == P);

            IEnumerable<CompPipe> pipes = cachedPipes.Where(x => x.PipeType == (PipeType)P && x.GridID == -1);
            foreach (var randomPipe in pipes)
            {
                PipelineNet newNet = Activator.CreateInstance(typeof(PipelineNet)) as PipelineNet;
                newNet.GasManager = this;
                newNet.NetID = masterID[P];
                newNet.NetType = (PipeType)P;
                PipeNets.Add(newNet);
                Predicate<IntVec3> predicate = delegate (IntVec3 c)
                {
                    foreach (var item in GridsUtility.GetThingList(c, map))
                    {
                        CompPipe compPipe2 = item.TryGetComp<CompPipe>();
                        if (compPipe2 != null && compPipe2.PipeType == (PipeType)P)
                        {
                            compPipe2.GridID = masterID[P];
                            compPipe2.pipeNet = newNet;
                            newNet.PipesThings.Add(compPipe2.parent);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, MapMeshFlag.Buildings);
                            map.mapDrawer.MapMeshDirty(compPipe2.parent.Position, MapMeshFlag.Things);
                            PipeGrid[P, map.cellIndices.CellToIndex(c)] = masterID[P];
                            return true;
                        }
                    }
                    return false;
                };

                map.floodFiller.FloodFill(randomPipe.parent.Position, predicate, delegate (IntVec3 x) { });
                masterID[P]++;
            }

            foreach (PipelineNet pipeNet in PipeNets)
            {
                pipeNet.InitNet();
            }
        }

        public bool PipeAt(IntVec3 pos, PipeType P)
        {
            return PipeGrid[(int)P, map.cellIndices.CellToIndex(pos)] >= 0;
        }
    }
}
