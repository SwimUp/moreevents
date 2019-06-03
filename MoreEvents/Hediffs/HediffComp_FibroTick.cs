using RimWorld;
using System.Linq;
using Verse;
using System.Collections.Generic;

namespace MoreEvents.Hediffs
{
    public class HediffComp_FibroTick : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            float percentSeverity = parent.Severity / parent.def.lethalSeverity;
            if (percentSeverity >= 0.98f && !parent.FullyImmune())
            {
                BodyPartRecord part = parent.Part;
                Pawn.TakeDamage(new DamageInfo(DamageDefOf.Frostbite,100, 0, -1, null, part));

                HediffSet set = Pawn.health.hediffSet;
                List<BodyPartRecord> allParts = (from x in Pawn.RaceProps.body.AllParts where !set.PartIsMissing(x) select x).ToList();
                if (allParts.TryRandomElement(out part))
                {
                    Pawn.health.AddHediff(HediffDefOfLocal.Fibrodysplasia, part);
                }
            }
        }
    }
}
