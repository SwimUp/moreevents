using DarkNET.TraderComp;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Traders
{
    public class TraderWorker_Sly : DarkNetTrader
    {
        private Vector2 slider = Vector2.zero;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        public enum Tab
        {
            Items,
            Service
        }

        private static Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        public List<SellableItemWithModif> StockForReading => stock;

        private List<SellableItemWithModif> stock;

        public override int ArriveTime => 5;

        public override int OnlineTime => 5;

        public DarkNetComp_Sly Comp
        {
            get
            {
                if (сomp == null)
                {
                    сomp = TryGetComp<DarkNetComp_Sly>();
                }

                return сomp;
            }
        }

        private DarkNetComp_Sly сomp;

        public override void FirstInit()
        {
            base.FirstInit();

            Inititialize();
        }

        private void Inititialize()
        {
            stock = new List<SellableItemWithModif>();
        }

        public override void Arrive()
        {
            base.Arrive();

            RegenerateStock();
        }


        public void RegenerateStock()
        {
            TryDestroyStock();

            int itemsCount = Comp.Props.CountRange.RandomInRange;
            float valueRange = Comp.Props.ValueRange.RandomInRange;

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();

            ThingSetMakerParams parms = default;
            parms.totalMarketValueRange = new FloatRange(valueRange, valueRange);
            parms.countRange = new IntRange(itemsCount, itemsCount);

            parms.filter = DarkNetPriceUtils.GetThingFilter(def.AvaliableGoods);

            maker.fixedParams = parms;

            var items = maker.Generate();
            stock = new List<SellableItemWithModif>();

            foreach (var item in items)
            {
                int itemValue = (int)((item.MarketValue * Character.Greed) * GetPriceModificatorByTechLevel(item.def.techLevel));
                if (!DarkNetPriceUtils.TryMerge(item, stock))
                {
                    stock.Add(new SellableItemWithModif(item, itemValue, null));
                }
            }
        }

        public void TryDestroyStock()
        {
            if (stock == null)
            {
                stock = new List<SellableItemWithModif>();
                return;
            }

            for (int num = stock.Count - 1; num >= 0; num--)
            {
                SellableItemWithModif item = stock[num];

                if (item.Item != null)
                {
                    Thing thing = item.Item;

                    if (!(thing is Pawn) && !thing.Destroyed)
                    {
                        thing.Destroy();
                    }
                }

                stock.Remove(item);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref stock, "stock", LookMode.Deep);
        }

        public override void DrawTraderShop(Rect rect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = rect;
            rect2.yMin += 35;

            Widgets.DrawMenuSection(rect2);
            tabsList.Clear();
            tabsList.Add(new TabRecord("TraderWorker_Sly_Tab_Items".Translate(), delegate
            {
                tab = Tab.Items;
            }, tab == Tab.Items));
            tabsList.Add(new TabRecord("TraderWorker_Sly_Tab_Weapons".Translate(), delegate
            {
                tab = Tab.Service;
            }, tab == Tab.Service));
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 450);

            switch (tab)
            {
                case Tab.Items:
                    {
                        DrawItems(tab, rect2);
                        break;
                    }
                case Tab.Service:
                    {
                        break;
                    }
            }

            Text.Font = GameFont.Small;
        }

        private void DrawItems(Tab tab, Rect rect)
        {
            List<SellableItemWithModif> items = stock;

            Rect goodsRect = new Rect(rect.x + 10, rect.y + 5, rect.width - 15, rect.height - 25);
            Rect goodRect = new Rect(0, 0, goodsRect.width - 25, 200);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, items.Count * 205);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for (int i = 0; i < items.Count; i++)
            {
                SellableItemWithModif item = items[i];

                GUIUtils.DrawItemCard(item, items, goodRect);
                goodRect.y += 205;
            }
            Widgets.EndScrollView();
        }

        public float GetPriceModificatorByTechLevel(TechLevel level)
        {
            switch (level)
            {
                case TechLevel.Undefined:
                    return 1f;
                case TechLevel.Animal:
                    return 0.7f;
                case TechLevel.Neolithic:
                    return 1.15f;
                case TechLevel.Medieval:
                    return 1.25f;
                case TechLevel.Industrial:
                    return 1.3f;
                case TechLevel.Spacer:
                    return 1.5f;
                case TechLevel.Ultra:
                    return 2f;
                default:
                    return 1f;
            }
        }

    }
}
