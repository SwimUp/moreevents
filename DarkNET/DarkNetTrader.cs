using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public abstract class DarkNetTrader : IExposable
    {
        public DarkNetTraderDef def;

        public string Name => def.label;

        public string Description => def.description;

        public Texture2D IconMenu => def.IconTexture;

        public virtual int OnlineTime => 2;
        public virtual int ArriveTime => 10;

        private int lastArriveTicks = 0;

        public bool Online;

        public virtual bool OnlineEveryTime => false;

        public virtual void FirstInit()
        {
            RegenerateStock();
        }

        public virtual void Arrive()
        {

        }

        public virtual void OnDayPassed()
        {
            if (OnlineEveryTime)
                return;

            float num = (Find.TickManager.TicksGame - lastArriveTicks) / 60000f;
            if (Online)
            {
                if (num > OnlineTime)
                {
                    Online = false;
                    lastArriveTicks = Find.TickManager.TicksGame;
                }
            }
            else
            {
                if (num > ArriveTime)
                {
                    Online = true;
                    lastArriveTicks = Find.TickManager.TicksGame;
                    Arrive();
                }
            }
        }

        public abstract void DrawTraderShop(Rect rect);

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref lastArriveTicks, "lastArriveTicks");
            Scribe_Values.Look(ref Online, "Online");
        }
    }
}
