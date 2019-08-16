using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class GasManager : MapComponent
    {
        public Dictionary<PipeType, List<PipelineNet>> PipelineSet;
        public PipelineNet[] PipeNets;

        public List<PipelineNet> AllPipeNets = new List<PipelineNet>();

        public int[,] PipeGrid;

        public GasManager(Map map) : base(map)
        {
            int length = Enum.GetValues(typeof(PipeType)).Length;
            PipeGrid = new int[length, map.cellIndices.NumGridCells];

            PipelineSet = new Dictionary<PipeType, List<PipelineNet>>();
            for (int i = 0; i < length; i++)
            {
                PipelineSet.Add((PipeType)i, new List<PipelineNet>());

                for (int i2 = 0; i2 < PipeGrid.GetLength(1); i2++)
                {
                    PipeGrid[i, i2] = -1;
                }
            }

            PipeNets = new PipelineNet[map.cellIndices.NumGridCells];
        }

        public void RegisterPipe(CompPipe pipe, bool respawningAfterLoad)
        {
            foreach (IntVec3 item in GenAdj.CellsAdjacentCardinal(pipe.parent))
            {
                TryDestroyPipeNet(item, pipe.PipeType);
            }

            TryCreatePipeNet(pipe.parent.Position, pipe.PipeType);
        }

        public void TryCreatePipeNet(IntVec3 pipe, PipeType type)
        {
            if (!pipe.InBounds(map) || PipeNetAt(pipe) != null)
                return;

            int pipeTypeInt = (int)type;

            PipelineNet newNet = new PipelineNet
            {
                GasManager = this,
                NetType = type
            };

            Predicate<IntVec3> predicate = delegate (IntVec3 c)
            {
                foreach (var item in GridsUtility.GetThingList(c, map))
                {
                    CompPipe compPipe2 = item.TryGetComp<CompPipe>();
                    if (compPipe2 != null)
                    {
                        if (compPipe2.PipeType == type)
                        {
                            compPipe2.pipeNet = newNet;
                            newNet.PipesThings.Add(compPipe2.parent);

                            CellRect cellRect = compPipe2.parent.OccupiedRect();
                            foreach (var cell in cellRect)
                            {
                                int num = map.cellIndices.CellToIndex(cell.x, cell.z);
                                PipeGrid[pipeTypeInt, map.cellIndices.CellToIndex(c)] = 1;
                                PipeNets[num] = newNet;
                            }
                        }

                        if(compPipe2.GasProps.ConnectEverything)
                        {
                            newNet.PipesThings.Add(compPipe2.parent);
                        }
                        return true;
                    }
                }
                return false;
            };

            map.floodFiller.FloodFill(pipe, predicate, delegate (IntVec3 x) { });
            PipelineSet[type].Add(newNet);
            AllPipeNets.Add(newNet);

            newNet.InitNet();
        }

        public void TryDestroyPipeNet(IntVec3 cell, PipeType matchingType)
        {
            if(cell.InBounds(map))
            {
                var pipeNet = PipeNetAt(cell);
                if (pipeNet != null && pipeNet.NetType == matchingType)
                {
                    DeletePipeNet(pipeNet);
                }
            }
        }

        public void DeletePipeNet(PipelineNet pipeNet)
        {
            AllPipeNets.Remove(pipeNet);
            PipelineSet[pipeNet.NetType].Remove(pipeNet);

            int pipeType = (int)pipeNet.NetType;
            foreach(var pipeThing in pipeNet.PipesThings)
            {
                CellRect cellRect = pipeThing.OccupiedRect();
                foreach (var cell in cellRect)
                {
                    int num = map.cellIndices.CellToIndex(cell);
                    PipeNets[num] = null;
                    PipeGrid[pipeType, num] = -1;
                }
            }
        }

        public void DeregisterPipe(CompPipe pipe)
        {
            TryDestroyPipeNet(pipe.parent.Position, pipe.PipeType);

            foreach (IntVec3 item in GenAdj.CellsAdjacentCardinal(pipe.parent.Position, pipe.parent.Rotation, pipe.parent.def.size))
            {
                TryCreatePipeNet(item, pipe.PipeType);
            }
        }
        public bool PipeAt(IntVec3 pos, PipeType P)
        {
            return PipeGrid[(int)P, map.cellIndices.CellToIndex(pos)] == 1;
        }

        public bool AnyPipeAt(IntVec3 pos)
        {
            for(int i = 0; i < PipeGrid.GetLength(0); i++)
            {
                if(PipeGrid[i, map.cellIndices.CellToIndex(pos)] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public PipelineNet PipeNetAt(IntVec3 c)
        {
            return PipeNets[map.cellIndices.CellToIndex(c)];
        }
    }
}
