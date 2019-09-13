using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffCompProperties_ApplyHediffAfter : HediffCompProperties
    {
        public HediffDef Hediff;

        public float Severity = 1f;

        public HediffCompProperties_ApplyHediffAfter()
        {
            compClass = typeof(HediffComp_ApplyHediffAfter);
        }
    }
}
