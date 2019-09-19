using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class CompProperties_ElectricUnbreakable : CompProperties
    {
        public CompProperties_ElectricUnbreakable()
        {
            compClass = typeof(CompElectricUnbreakable);
        }
    }
}
