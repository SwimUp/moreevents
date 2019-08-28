using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class PriceModificatorFilter
    {
        public List<PriceModificatorDef> AllowedPriceModificators;

        public List<string> AllowedPriceModificatorsCategory;

        public void ResolveReferences()
        {
            if(AllowedPriceModificators == null)
            {
                AllowedPriceModificators = new List<PriceModificatorDef>();
            }

            if(AllowedPriceModificatorsCategory != null)
            {
                foreach(var modificator in DefDatabase<PriceModificatorDef>.AllDefs)
                {
                    foreach(var cat in AllowedPriceModificatorsCategory)
                    {
                        if(modificator.Categories != null && modificator.Categories.Contains(cat))
                        {
                            AllowedPriceModificators.Add(modificator);
                        }
                    }
                }
            }
        }
    }
}
