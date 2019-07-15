using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffCompProperties_GiveMentalBreak : HediffCompProperties
    {
        public MentalBreakDef MentalBreak;

        public float MinSeverity = -1; //-1 auto

        public HediffCompProperties_GiveMentalBreak()
        {
            compClass = typeof(HediffComp_GiveMentalBreak);
        }
    }
}
