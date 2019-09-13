using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffCompProperties_UseCooldown : HediffCompProperties
    {
        public int DaysCooldown;

        public HediffDef AppendHediff;

        public HediffCompProperties_UseCooldown()
        {
            compClass = typeof(HediffComp_UseCooldown);
        }
    }
}
