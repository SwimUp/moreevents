using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Traders
{
    public class TraderWorker_RogerEdmonson : DarkNetTrader
    {
        public override int OnlineTime => 1;

        public override int ArriveTime => 2;

        private SimpleCurve itemsCountPerRaidCurve = new SimpleCurve
        {
            new CurvePoint(0, 3),
            new CurvePoint(2, 5),
            new CurvePoint(5, 10),
            new CurvePoint(10, 15),
            new CurvePoint(14, 20)
        };

        private float marketValueMultiplierPerMapEvent => 80;

        private float startMarketValue => 1200;

        private int lastRaidsEnemy = 0;

        public override void DrawTraderShop(Rect rect)
        {
            Rect imageRect = new Rect(700, rect.y, 400, rect.height);
            GUI.DrawTexture(imageRect, def.FullTexture);

            Rect specialGoodsRect = new Rect(rect.x + 2, rect.y + 2, 580, 30);
            Widgets.Label(specialGoodsRect, "RogerEdmonson_SpecialGoods".Translate());

            GUIUtils.DrawLineHorizontal(rect.x, 200, 600, Color.gray);
        }

        public override void Arrive()
        {
            RegenerateStock();
        }

        public virtual void RegenerateStock()
        {
            TryDestroyStock();

            int raidsCount = Find.StoryWatcher.statsRecord.numRaidsEnemy - lastRaidsEnemy;
            lastRaidsEnemy = raidsCount;

            int itemsCount = (int)itemsCountPerRaidCurve.Evaluate(raidsCount);
        }

        public void TryDestroyStock()
        {
            if (stock == null)
            {
                return;
            }
            for (int num = stock.Count - 1; num >= 0; num--)
            {
                Thing thing = stock[num];
                stock.Remove(thing);
                if (!(thing is Pawn) && !thing.Destroyed)
                {
                    thing.Destroy();
                }
            }
            stock = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastRaidsEnemy, "lastRaidsEnemy");
        }
    }
}
