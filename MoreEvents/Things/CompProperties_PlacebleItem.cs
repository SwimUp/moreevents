using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things
{
    public class CompProperties_PlacebleItem : CompProperties
    {
        public ThingDef PlaceDef;
        public CompProperties_PlacebleItem()
        {
            compClass = typeof(PlacebleItem);
        }
    }
}
