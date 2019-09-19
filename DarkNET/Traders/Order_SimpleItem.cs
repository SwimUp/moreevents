using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Traders
{
    public class Order_SimpleItem : Order
    {
        public ThingDef Item;

        public int Count;

        public Order_SimpleItem()
        {

        }

        public Order_SimpleItem(float chance, float price, int delay, ThingDef item, int count)
        {
            Delay = delay;
            Price = price;
            Chance = chance;
            Item = item;
            Count = count;
        }

        public override Thing GenerateItem(DarkNetTrader trader)
        {
            Thing newThing = ThingMaker.MakeThing(Item);
            newThing.stackCount = Count;

            return newThing;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref Item, "Item");
            Scribe_Values.Look(ref Count, "Count");
        }
    }
}
