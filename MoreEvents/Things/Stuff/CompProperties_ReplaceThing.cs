using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompProperties_ReplaceThing : CompProperties
    {
        public ThingDef ReplaceThing;

        public ThingDef CompareThing;

        public CompProperties_ReplaceThing()
        {
            compClass = typeof(CompReplaceThing);
        }
    }
}
