using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class CompProperties_Overdose : CompProperties
    {
        public FloatRange overdoseSeverityOffset = FloatRange.Zero;

        public float largeOverdoseChance;

        public CompProperties_Overdose()
        {
            compClass = typeof(CompOverdose);
        }
    }
}
