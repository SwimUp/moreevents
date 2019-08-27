using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Traders
{
    public class Order_BodyPart_RogerEdmonson : Order
    {
        public OrderBodypartGroup Group;

        public Order_BodyPart_RogerEdmonson()
        {

        }

        public Order_BodyPart_RogerEdmonson(float chance, float price, int delay, OrderBodypartGroup group)
        {
            Chance = chance;
            Price = price;
            Delay = delay;
            Group = group;
        }

        public override Thing GenerateItem(DarkNetTrader trader)
        {
            TraderWorker_RogerEdmonson roger = (TraderWorker_RogerEdmonson)trader;

            if(roger.OrderBodyparts.Where(item => item.BodypartGroup == Group).TryRandomElementByWeight(x => x.Commonality, out OrderItem result))
            {
                Thing thing = ThingMaker.MakeThing(result.ThingDef);
                if(PriceModificatorUtils.TryGetPriceModificator(thing, roger.def, out PriceModificatorDef modificator))
                {
                    DarkNetPriceUtils.FinalizeItem(thing, modificator);
                }

                return thing;
            }

            return null;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Group, "Group");
        }
    }
}
