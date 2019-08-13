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

        public bool Enabled => compFlickable.SwitchIsOn && HasPower && pipeNet.Enabled;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            ParentGasWell = GridsUtility.GetFirstThingWithComp<CompGasWell>(parent.Position, parent.Map).GetComp<CompGasWell>();
        }

        public override void PipelineNet()
        {
            
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            if(Enabled)
            {
                builder.Append("GasPlant_ActiveInfo");
            }
            else
            {
                builder.Append("GasPlant_ActiveInfoOFF");
            }
            builder.Append("GasPlant_GasWellInfo".Translate(ParentGasWell.GasReserves.ToString("f2")));

            return base.CompInspectStringExtra();
        }
    }
}
