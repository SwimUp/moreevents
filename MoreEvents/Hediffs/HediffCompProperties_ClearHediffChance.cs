using DarkNET.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffCompProperties_ClearHediffChance : HediffCompProperties
    {
        public float ChanceWhenUse;

        public List<HediffDef> ClearHeddifs;

        public HediffCompProperties_ClearHediffChance()
        {
            compClass = typeof(HediffComp_ClearHediffChance);
        }
    }
}
