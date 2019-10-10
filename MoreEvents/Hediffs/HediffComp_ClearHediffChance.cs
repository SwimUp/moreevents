using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffComp_ClearHediffChance : HediffComp
    {
        public HediffCompProperties_ClearHediffChance Props => (HediffCompProperties_ClearHediffChance)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Use();
        }

        public override void CompPostMerged(Hediff other)
        {
            Use();
        }

        private void Use()
        {
            if (Rand.Chance(Props.ChanceWhenUse))
            {
                foreach (var hediffDef in Props.ClearHeddifs)
                {
                    Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (hediff != null)
                    {
                        Pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}
