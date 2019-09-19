using MoreEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Pawns
{
    public class Pawn_Termitnator : Pawn
    {
        public int lastHealTicks = 0;

        public int healCooldown => 25000;

        public HediffDef Hediff => HediffDefOfLocal.BioChemicRegeneration;

        public override void Tick()
        {
            base.Tick();

            if (Find.TickManager.TicksGame % 150 == 0)
            {
                if (Find.TickManager.TicksGame - lastHealTicks > healCooldown)
                {
                    if (health.hediffSet.GetInjuriesTendable().Any() && health.hediffSet.GetFirstHediffOfDef(Hediff) == null)
                    {
                        HealthUtility.AdjustSeverity(this, Hediff, 1f);
                        lastHealTicks = Find.TickManager.TicksGame;
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastHealTicks, "lastHealTicks");
        }
    }
}
