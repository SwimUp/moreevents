using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasCooler : CompProperties_GasPipe
    {
        public float CoolingRate;

        public CompProperties_GasCooler()
        {
            compClass = typeof(CompGasCooler);
        }
    }
}
