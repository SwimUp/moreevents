using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class SellableItemWithModif : IExposable
    {
        public Thing Item;
        public int MarketValue;

        public PriceModificatorDef Modificator;
        public SellableItemWithModif()
        {

        }

        public SellableItemWithModif(Thing item, int value, PriceModificatorDef modificator)
        {
            Item = item;
            MarketValue = value;
            Modificator = modificator;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Item, "Item");
            Scribe_Values.Look(ref MarketValue, "MarketValue");
            Scribe_Defs.Look(ref Modificator, "Modificator");
        }
    }
}
