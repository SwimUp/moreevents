using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasPowerPlantSettings : CompPipe
    {
        public CompProperties_GasPowerPlant Props => (CompProperties_GasPowerPlant)props;

        public CompGasPowerPlant compGasPowerPlant => parent.GetComp<CompGasPowerPlant>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void NetInit()
        {
            base.NetInit();

        }
    }
}
