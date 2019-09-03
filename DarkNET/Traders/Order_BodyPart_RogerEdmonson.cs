using RimWorld;
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

        public override void TraderArrive(DarkNetTrader trader)
        {
            base.TraderArrive(trader);
        }

        public override Thing GenerateItem(DarkNetTrader trader)
        {
            TraderWorker_RogerEdmonson roger = (TraderWorker_RogerEdmonson)trader;

            if (roger.OrderBodyparts.First(x => x.BodypartGroup == Group).Items.TryRandomElementByWeight(x2 => x2.Commonality, out OrderThing result))
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

        public override void Fail(DarkNetTrader trader)
        {
            TraderWorker_RogerEdmonson roger = (TraderWorker_RogerEdmonson)trader;

            Find.LetterStack.ReceiveLetter("Order_BodyPart_RogerEdmonson_FailTitle".Translate(), "Order_BodyPart_RogerEdmonson_FailDesc".Translate($"{Group}_group".Translate()), LetterDefOf.NegativeEvent);

            roger.Order = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Group, "Group");
        }
    }
}
