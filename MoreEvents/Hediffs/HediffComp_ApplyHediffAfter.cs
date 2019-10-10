using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffComp_ApplyHediffAfter : HediffComp
    {
        public HediffCompProperties_ApplyHediffAfter Props => (HediffCompProperties_ApplyHediffAfter)props;

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            if (Pawn == null || Pawn.Dead)
                return;

            HealthUtility.AdjustSeverity(Pawn, Props.Hediff, Props.Severity);
        }
    }
}
