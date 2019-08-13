using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasWell : CompProperties
    {
        public FloatRange GasRange;

        public CompProperties_GasWell()
        {
            compClass = typeof(CompGasWell);
        }
    }
}
