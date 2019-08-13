using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasPlant : CompProperties_GasPipe
    {
        public float PumpingSpeed;

        public CompProperties_GasPlant()
        {
            compClass = typeof(GasPlant);
        }
    }
}
