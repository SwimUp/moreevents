using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Traders
{
    public class Order_Art_RogerEdmonson : Order
    {
        public ThingDef Art;

        public ThingDef Stuff;

        public Order_Art_RogerEdmonson()
        {

        }

        public Order_Art_RogerEdmonson(float chance, float price, int delay, ThingDef art, ThingDef stuff)
        {
            Delay = delay;
            Price = price;
            Chance = chance;
            Art = art;
            Stuff = stuff;
        }

        public override Thing GenerateItem(DarkNetTrader trader)
        {
            TraderWorker_RogerEdmonson roger = (TraderWorker_RogerEdmonson)trader;

            Thing newArt = ThingMaker.MakeThing(Art, Stuff);
            newArt.TryGetComp<CompQuality>()?.SetQuality(QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Colony);
            newArt = newArt.MakeMinified();

            return newArt;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref Art, "art");
            Scribe_Defs.Look(ref Stuff, "Stuff");
        }
    }
}
