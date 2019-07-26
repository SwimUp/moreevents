using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class CompProperties_ArmorCore : CompProperties
    {
        public float PowerCapacity;
        public string StationLabel;

        public CompProperties_ArmorCore()
        {
            compClass = typeof(ArmorCore);
        }
    }
}
