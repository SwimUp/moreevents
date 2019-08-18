using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasPowerPlant : CompProperties_GasPipe
    {
        public float GasConsumption;

        public CompProperties_GasPowerPlant()
        {
            compClass = typeof(CompGasPowerPlantSettings);
        }
    }
}
