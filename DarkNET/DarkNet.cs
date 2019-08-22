using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class DarkNet : GameComponent
    {
        public List<DarkNetTrader> Traders;

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            if (Traders == null)
            {
                InitDarkNet();
            }
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            if (Traders == null)
            {
                InitDarkNet();
            }
        }

        public void InitDarkNet()
        {
            Traders = new List<DarkNetTrader>();

            foreach(var trader in DefDatabase<DarkNetTraderDef>.AllDefs)
            {
                DarkNetTrader newTrader = CreateAndInitTrader(trader);

                Traders.Add(newTrader);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref Traders, "Traders");
        }

        private DarkNetTrader CreateAndInitTrader(DarkNetTraderDef def)
        {
            DarkNetTrader newTrader = new DarkNetTrader();

            newTrader.def = def;

            return newTrader;
        }
    }
}
