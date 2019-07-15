using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffCompProperties_AutoHeal : HediffCompProperties
    {
        public float HealModiff = 1f;

        public int HealTicks = 6000;

        public HediffCompProperties_AutoHeal()
        {
            compClass = typeof(HediffComp_AutoHeal);
        }
    }
}
