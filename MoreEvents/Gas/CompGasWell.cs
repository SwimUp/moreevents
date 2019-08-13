using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasWell : ThingComp
    {
        public CompProperties_GasWell Props => (CompProperties_GasWell)props;

        public float GasReserves = 0f;
        private bool init;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if(!init)
            {
                GasReserves = Props.GasRange.RandomInRange;
                init = true;
            }

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref GasReserves, "GasReserves");
            Scribe_Values.Look(ref init, "init");
        }

        public override string CompInspectStringExtra()
        {
            return "CompGasWell_GasReserve".Translate(GasReserves.ToString("f2"));
        }
    }
}
