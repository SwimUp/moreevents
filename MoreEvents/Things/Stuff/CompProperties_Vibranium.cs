using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompProperties_Vibranium : CompProperties
    {
        public Dictionary<DamageDef, float> AbsorbDamage;

        public CompProperties_Vibranium()
        {
            compClass = typeof(CompVibranium);
        }
    }
}
