using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Traders
{
    public abstract class Order : IExposable
    {
        public int Delay;

        public float Chance;

        public float Price;

        public bool Finish;

        public bool Success => OrderedItem != null;

        public Thing OrderedItem;

        public virtual void TraderArrive(DarkNetTrader trader)
        {
            if (Finish)
                return;

            Delay--;

            if (Delay == 0)
            {
                Finish = true;
                if (Rand.Chance(Chance))
                {
                    OrderedItem = GenerateItem(trader);
                }
            }
        }

        public abstract Thing GenerateItem(DarkNetTrader trader);

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref Delay, "Delay");
            Scribe_Values.Look(ref Chance, "Chance");
            Scribe_Values.Look(ref Price, "Price");
            Scribe_Deep.Look(ref OrderedItem, "OrderedItem");
            Scribe_Values.Look(ref Finish, "Finish");
        }
    }
}
