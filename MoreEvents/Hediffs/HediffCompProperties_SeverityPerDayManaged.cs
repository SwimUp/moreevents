using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Hediffs
{
    public class HediffCompProperties_SeverityPerDayManaged : HediffCompProperties
    {
        public float severityPerDay;

        public HediffCompProperties_SeverityPerDayManaged()
        {
            compClass = typeof(HediffComp_SeverityPerDayManaged);
        }
    }
}
