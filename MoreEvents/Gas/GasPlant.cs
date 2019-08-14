using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class GasPlant : CompPipe
    {
        public CompProperties_GasPlant Props => (CompProperties_GasPlant)props;

        public CompGasWell ParentGasWell = null;

        public bool Enabled => HasPower;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            ParentGasWell = GridsUtility.GetFirstThingWithComp<CompGasWell>(parent.Position, parent.Map).GetComp<CompGasWell>();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!HasPower)
                return;

            float toPump = Mathf.Min(Props.PumpingSpeed, ParentGasWell.GasReserves);
            pipeNet.PushGasIntoNet(this, toPump);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            if (DebugSettings.godMode)
            {
                builder.AppendLine(base.CompInspectStringExtra());
            }

            if (Enabled)
            {
                builder.Append("GasPlant_ActiveInfo".Translate());
            }
            else
            {
                builder.Append("GasPlant_ActiveInfoOFF".Translate());
            }
            builder.Append("GasPlant_GasWellInfo".Translate(ParentGasWell.GasReserves.ToString("f2")));

            return builder.ToString();
        }
    }
}
