using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class PriceModificatorDef : Def
    {
        public float PriceModficator = 1.0f;

        public List<ThingCategoryDef> LinkedCategory;

        public List<ThingDef> SpecialThings;

        public float Commonality;

        public QualityRange QualityRange;

        public FloatRange HealthRange;

        public List<string> Categories;
    }
}
