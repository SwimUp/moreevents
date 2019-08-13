using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class GasPlant : CompPipe
    {
        public CompProperties_GasPlant Props => (CompProperties_GasPlant)props;

        public CompGasWell ParentGasWell = null;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            ParentGasWell = GridsUtility.GetFirstThingWithComp<CompGasWell>(parent.Position, parent.Map).GetComp<CompGasWell>();
        }

        public override void PipelineNet()
        {
            Log.Message($"tick {ParentGasWell.parent}");
        }
    }
}
