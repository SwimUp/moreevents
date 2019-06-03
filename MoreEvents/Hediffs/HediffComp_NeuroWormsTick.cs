using RimWorld;
using System.Linq;
using Verse;
using System.Collections.Generic;

namespace MoreEvents.Hediffs
{
    public class HediffComp_NeuroWormsTick : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            float percentSeverity = parent.Severity / parent.def.lethalSeverity;
            if (percentSeverity >= 0.98f && !parent.FullyImmune())
            {
                Hediff hediff = Pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == parent.def && x.Part == parent.Part && x.Visible);
                Pawn.health.RemoveHediff(hediff);
                Pawn.health.AddHediff(HediffDefOfLocal.NeurofibromatousWormsFinal, parent.Part);
            }
        }
    }
}
