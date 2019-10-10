using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffComp_RestoreBodypart : HediffComp
    {
        public HediffCompProperties_RestoreBodypart Props => (HediffCompProperties_RestoreBodypart)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            TryHealRandomPart();
        }

        public override void CompPostMerged(Hediff other)
        {
            TryHealRandomPart();
        }

        private void TryHealRandomPart()
        {
            float chance = Props.ChancePerSeverity.Evaluate(parent.Severity);

            if (Rand.Chance(chance))
            {
                List<BodyPartRecord> nonMissingParts = Pawn.RaceProps.body.AllParts.Where(x => Props.HealableParts.Contains(x.def) && (Pawn.health.hediffSet.PartIsMissing(x) || Pawn.health.hediffSet.hediffs.Any(x2 => x2.Part == x && !(x2 is Hediff_AddedPart)))).ToList();

                if (nonMissingParts.Count == 0)
                    return;

                if (nonMissingParts.TryRandomElement(out BodyPartRecord result))
                {
                    Pawn.health.RestorePart(result);
                }
            }
        }
    }
}
