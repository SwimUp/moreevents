using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class Building_Pipe : Building
    {
        public CompPipe CompPipe;

        private int pipeInt = 0;

        public override Graphic Graphic => GasGraphic.PipeGraphic[pipeInt];

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            CompPipe = GetComp<CompPipe>();
            pipeInt = (int)CompPipe.PipeType;

            base.SpawnSetup(map, respawningAfterLoad);
        }
    }
}
