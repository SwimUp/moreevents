using MoreEvents.MentalBreaks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffComp_GiveMentalBreak : HediffComp
    {
        private HediffCompProperties_GiveMentalBreak Props => (HediffCompProperties_GiveMentalBreak)props;

        private float minSeverity;

        public override void CompPostMake()
        {
            base.CompPostMake();

            if(Props.MinSeverity == -1)
            {
                minSeverity = parent.def.maxSeverity;
            }
            else
            {
                minSeverity = Props.MinSeverity;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.Severity >= minSeverity && !parent.FullyImmune())
            {
                Props.MentalBreak.Worker.TryStart(Pawn, parent.def.LabelCap, false);
                Pawn.health.RemoveHediff(parent);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref minSeverity, "severity");
        }
    }
}
