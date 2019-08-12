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

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            CompPipe = GetComp<CompPipe>();
        }

    }
}
