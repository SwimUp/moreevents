using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public static class PriceModificatorUtils
    {
        public static bool TryGetPriceModificator(Thing useItem, DarkNetTraderDef traderDef, out PriceModificatorDef modificator)
        {
            Thing item = useItem;

            modificator = null;

            MinifiedThing minifiedThing = item as MinifiedThing;
            if (minifiedThing != null)
            {
                useItem = minifiedThing.InnerThing;
            }

            if (traderDef.AllowedPriceModificatorsFilter == null || traderDef.AllowedPriceModificatorsFilter.AllowedPriceModificators == null)
                return false;

            List<PriceModificatorDef> allowedModificators = traderDef.AllowedPriceModificatorsFilter.AllowedPriceModificators.Where(delegate (PriceModificatorDef x)
            {
                if (x.SpecialThings != null && x.SpecialThings.Contains(item.def))
                    return true;

                if (x.LinkedCategory != null && item.def.thingCategories != null)
                {
                    foreach (var category in x.LinkedCategory)
                    {
                        if (item.def.thingCategories.Contains(category))
                            return true;
                    }
                }

                if(x.TradeCategories != null && item.def.tradeTags != null)
                {
                    foreach (var category in x.TradeCategories)
                    {
                        if (item.def.tradeTags.Contains(category))
                            return true;
                    }
                }

                return false;
            }).ToList();

            if (allowedModificators.Count == 0)
                return false;

            if (allowedModificators.TryRandomElementByWeight(x => x.Commonality, out modificator))
                return true;

            return false;
        }
    }
}
