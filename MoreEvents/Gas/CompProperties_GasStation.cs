using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompProperties_GasStation : CompProperties_GasPipe
    {
        public Dictionary<RecipeDef, float> GasModifier;

        public CompProperties_GasStation()
        {
            compClass = typeof(CompGasStation);
        }
    }
}
