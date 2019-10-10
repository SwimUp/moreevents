using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffComp_ClearHediffPerUse : HediffComp
    {
        public HediffCompProperties_ClearHediffPerUse Props => (HediffCompProperties_ClearHediffPerUse)props;

        private int uses;

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
            uses++;

            if (uses >= Props.Uses)
            {
                foreach (var hediffDef in Props.ClearHeddifs)
                {
                    Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (hediff != null)
                    {
                        Pawn.health.RemoveHediff(hediff);
                    }
                }

                uses = 0;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref uses, "uses");
        }
    }
}
