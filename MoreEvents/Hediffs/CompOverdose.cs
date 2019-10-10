using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class CompOverdose : ThingComp
    {
        public CompProperties_Overdose Props => (CompProperties_Overdose)props;

        public override void PostIngested(Pawn ingester)
        {
            float num2 = ingester.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.DrugOverdose)?.Severity ?? 0f;
            if (num2 < 0.9f && Rand.Value < Props.largeOverdoseChance)
            {
                float num3 = Rand.Range(0.85f, 0.99f);
                HealthUtility.AdjustSeverity(ingester, HediffDefOf.DrugOverdose, num3 - num2);
                if (ingester.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageAccidentalOverdose".Translate(ingester.Named("INGESTER"), parent.LabelNoCount, parent.Named("DRUG")).CapitalizeFirst(), ingester, MessageTypeDefOf.NegativeHealthEvent);
                }
            }
            else
            {
                float num4 = Props.overdoseSeverityOffset.RandomInRange / ingester.BodySize;
                if (num4 > 0f)
                {
                    HealthUtility.AdjustSeverity(ingester, HediffDefOf.DrugOverdose, num4);
                }
            }
        }
    }
}
