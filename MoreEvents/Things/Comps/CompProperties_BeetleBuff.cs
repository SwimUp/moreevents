using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Comps
{
    public class CompProperties_BeetleBuff : CompProperties
    {
        public float SpeedMultiplier = 1.2f;

        public float DamageMultiplier = 1.2f;

        public CompProperties_BeetleBuff()
        {
            compClass = typeof(CompBettleBuff);
        }
    }
}
