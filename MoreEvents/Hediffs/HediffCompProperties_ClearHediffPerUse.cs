using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffCompProperties_ClearHediffPerUse : HediffCompProperties
    {
        public int Uses;

        public List<HediffDef> ClearHeddifs;

        public HediffCompProperties_ClearHediffPerUse()
        {
            compClass = typeof(HediffComp_ClearHediffPerUse);
        }
    }
}
