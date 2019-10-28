using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffCompProperties_Explode : HediffCompProperties
    {
        public float ExplodeSeverity;

        public bool RemoveHediff;

        public HediffCompProperties_Explode()
        {
            compClass = typeof(HediffComp_Explode);
        }
    }
}
