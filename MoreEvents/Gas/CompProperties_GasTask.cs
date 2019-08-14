using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasTask : CompProperties_GasPipe
    {
        public float StorageCapacity;

        public CompProperties_GasTask()
        {
            compClass = typeof(CompGasTank);
        }
    }
}
