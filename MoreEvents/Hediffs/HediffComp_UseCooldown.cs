using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffComp_UseCooldown : HediffComp
    {
        public HediffCompProperties_UseCooldown Props => (HediffCompProperties_UseCooldown)props;

        private int lastUseTicks;

        public override void CompPostMerged(Hediff other)
        {
            CheckState();
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            CheckState();
        }

        private void CheckState()
        {
            if (lastUseTicks == 0)
            {
                lastUseTicks = Find.TickManager.TicksGame;
                return;
            }

            float num = (float)(Find.TickManager.TicksGame - lastUseTicks) / 60000f;
            if (num < Props.DaysCooldown)
            {
                HealthUtility.AdjustSeverity(Pawn, Props.AppendHediff, 0.5f);
                lastUseTicks = Find.TickManager.TicksGame;
                return;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref lastUseTicks, "lastUseTicks");
        }
    }
}
