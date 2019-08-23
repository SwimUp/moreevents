﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class DarkNet : GameComponent
    {
        public List<DarkNetTrader> Traders;

        public DarkNet()
        {

        }

        public DarkNet(Game game)
        {

        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            if (Traders == null)
            {
                InitDarkNet();
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (Traders == null)
                return;

            if(Find.TickManager.TicksGame % 60000 == 0)
            {
                foreach(var trader in Traders)
                {
                    trader.OnDayPassed();
                }
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

            Scribe_Collections.Look(ref Traders, "Traders", LookMode.Deep);
        }

        private DarkNetTrader CreateAndInitTrader(DarkNetTraderDef def)
        {
            DarkNetTrader newTrader = (DarkNetTrader)Activator.CreateInstance(def.workerClass);

            newTrader.def = def;

            newTrader.FirstInit();

            return newTrader;
        }
    }
}
