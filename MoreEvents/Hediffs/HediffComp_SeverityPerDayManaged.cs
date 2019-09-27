using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Hediffs
{
    public class HediffComp_SeverityPerDayManaged : HediffComp
    {
        protected const int SeverityUpdateInterval = 200;

        public bool AddSeverity = true;

        private HediffCompProperties_SeverityPerDayManaged Props => (HediffCompProperties_SeverityPerDayManaged)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (base.Pawn.IsHashIntervalTick(200))
            {
                float num = SeverityChangePerDay();
                num *= 0.00333333341f;
                if(AddSeverity)
                    severityAdjustment += num;
                else
                    severityAdjustment -= num;
            }
        }

        protected virtual float SeverityChangePerDay()
        {
            return Props.severityPerDay;
        }

        public override string CompDebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.CompDebugString());
            if (!base.Pawn.Dead)
            {
                stringBuilder.AppendLine("severity/day: " + SeverityChangePerDay().ToString("F3"));
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref AddSeverity, "Add");
        }
    }
}
