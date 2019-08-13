using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasCooler : CompPipe
    {
        public List<CompPipe> OutPipes = new List<CompPipe>();
        public List<CompPipe> InPipes = new List<CompPipe>();

        public bool Enabled;

        public override void NetInit()
        {
            base.NetInit();

            OutPipes.Clear();
            InPipes.Clear();

            CellRect vals = GenAdj.OccupiedRect(parent.Position, Rot4.Invalid, new IntVec2(4,4));
            foreach(var cell in vals)
            {
                var thing = cell.GetFirstThing<Building_Pipe>(parent.Map);
                if(thing != null)
                {
                    var compPipe = thing.TryGetComp<CompPipe>();
                    if(compPipe.PipeType == PipeType.NaturalGas && !InPipes.Contains(compPipe) && !InPipes.Any(x => compPipe.pipeNet != null && x.pipeNet.NetID == compPipe.pipeNet.NetID))
                    {
                        InPipes.Add(compPipe);
                    }else if(compPipe.PipeType == PipeType.LiquidGas && !OutPipes.Contains(compPipe) && !OutPipes.Any(x => compPipe.pipeNet != null && x.pipeNet.NetID == compPipe.pipeNet.NetID))
                    {
                        OutPipes.Add(compPipe);
                    }
                }
            }

            if(InPipes.Count > 0 && OutPipes.Count > 0)
            {
                Enabled = true;
            }
        }
    }
}
