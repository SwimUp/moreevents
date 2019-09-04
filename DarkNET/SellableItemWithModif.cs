using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class SellableItemWithModif : IExposable
    {
        public Thing Item;
        public int MarketValue;

        public int CountToTransfer;
        public string EditBuffer;

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

        public void AddToTransfer(int count)
        {
            if (count > Item.stackCount)
                count = Item.stackCount;

            CountToTransfer = Mathf.Clamp(CountToTransfer + count, 0, Item.stackCount);
            EditBuffer = CountToTransfer.ToString();
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Item, "Item");
            Scribe_Values.Look(ref MarketValue, "MarketValue");
            Scribe_Defs.Look(ref Modificator, "Modificator");
        }
    }
}
