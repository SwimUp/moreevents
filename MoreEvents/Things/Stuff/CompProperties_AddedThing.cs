using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompProperties_AddedThing : CompProperties
    {
        public bool ForApparel = false;

        public bool ForWeapons = false;

        public Type Comp;

        public CompProperties_AddedThing()
        {
            compClass = typeof(CompAddedComp);
        }
    }
}
