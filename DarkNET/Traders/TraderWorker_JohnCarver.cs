using DarkNET.TraderComp;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static DarkNET.TraderComp.DarkNetProperties_JohnCarver;

namespace DarkNET.Traders
{
    public class TraderWorker_JohnCarver : DarkNetTrader
    {
        private Vector2 slider = Vector2.zero;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        public enum Tab
        {
            HardMetals,
            Experimental,
            Precious,
            Weapons
        }

        private static Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private List<CategoryItem> stock;
        public List<CategoryItem> Stock => stock;

        public override int ArriveTime => 1;

        public override int OnlineTime => 1;

        public class CategoryItem : IExposable
        {
            public Tab Tab;

            public List<SellableItemWithModif> Items;

            public void ExposeData()
            {
                Scribe_Values.Look(ref Tab, "Tab");
                Scribe_Collections.Look(ref Items, "Items", LookMode.Deep);
            }
        }

        public DarkNetComp_JohnCarver Comp
        {
            get
            {
                if (сomp == null)
                {
                    сomp = TryGetComp<DarkNetComp_JohnCarver>();
                }

                return сomp;
            }
        }

        private DarkNetComp_JohnCarver сomp;

        public override void FirstInit()
        {
            base.FirstInit();

            Inititialize();
        }

        private void Inititialize()
        {
            stock = new List<CategoryItem>();

            foreach (Tab cat in Enum.GetValues(typeof(Tab)))
            {
                CategoryItem catItem = new CategoryItem
                {
                    Tab = cat,
                    Items = new List<SellableItemWithModif>()
                };

                stock.Add(catItem);
            }
        }

        public override void Arrive()
        {
            base.Arrive();

            RegenerateStock();
        }


        public void RegenerateStock()
        {
            TryDestroyStock();

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms = default;

            foreach (var cat in stock)
            {
                CategoryItemSetting settings = Comp.Props.CategoryItemSettings.First(x => x.Tab == cat.Tab);

                if (cat.Items == null)
                    cat.Items = new List<SellableItemWithModif>();

                int itemsCount = settings.CountRange.RandomInRange;
                float valueRange = settings.ValueRange.RandomInRange * itemsCount;

                parms.totalMarketValueRange = new FloatRange(valueRange, valueRange);
                parms.countRange = new IntRange(itemsCount, itemsCount);

                ThingFilter filter = DarkNetPriceUtils.GetThingFilter(settings.Goods);
                if (filter.AllowedDefCount == 0)
                    continue;

                parms.filter = filter;

                maker.fixedParams = parms;

                var items = maker.Generate();

                foreach (var item in items)
                {
                    if (!TryMerge(item, cat.Items))
                    {
                        int marketValue = (int)((item.MarketValue * Character.Greed) * settings.PriceMultiplier);

                        SellableItemWithModif newItem = new SellableItemWithModif(item, marketValue, null);

                        cat.Items.Add(newItem);
                    }
                }
            }
        }

        private bool TryMerge(Thing item, List<SellableItemWithModif> stock)
        {
            for (int i = 0; i < stock.Count; i++)
            {
                Thing stockItem = stock[i].Item;
                if (!stockItem.CanStackWith(item))
                    continue;

                stockItem.stackCount += item.stackCount;

                item.Destroy();

                return true;
            }

            return false;
        }

        public void TryDestroyStock()
        {
            if (stock == null)
            {
                Inititialize();
                return;
            }

            foreach (var stockItem in stock)
            {
                for (int num2 = stockItem.Items.Count - 1; num2 >= 0; num2--)
                {
                    SellableItemWithModif item = stockItem.Items[num2];

                    if (item.Item != null)
                    {
                        Thing thing = item.Item;

                        if (!(thing is Pawn) && !thing.Destroyed)
                        {
                            thing.Destroy();
                        }
                    }
                }

                stockItem.Items = null;
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
            tabsList.Add(new TabRecord("TraderWorker_JohnCarver_Tab_HardMetals".Translate(), delegate
            {
                tab = Tab.HardMetals;
            }, tab == Tab.HardMetals));
            tabsList.Add(new TabRecord("TraderWorker_JohnCarver_Tab_Experimental".Translate(), delegate
            {
                tab = Tab.Experimental;
            }, tab == Tab.Experimental));
            tabsList.Add(new TabRecord("TraderWorker_JohnCarver_Tab_Precious".Translate(), delegate
            {
                tab = Tab.Precious;
            }, tab == Tab.Precious));
            tabsList.Add(new TabRecord("TraderWorker_JohnCarver_Tab_Weapons".Translate(), delegate
            {
                tab = Tab.Weapons;
            }, tab == Tab.Weapons));
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 200);

            DrawItems(tab, rect2);

            Text.Font = GameFont.Small;
        }

        private void DrawItems(Tab tab, Rect rect)
        {
            List<SellableItemWithModif> items = stock.First(x => x.Tab == tab).Items;

            Rect goodsRect = new Rect(rect.x + 10, rect.y + 5, rect.width - 15, rect.height - 25);
            Rect goodRect = new Rect(0, 0, goodsRect.width - 25, 200);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, items.Count * 205);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for (int i = 0; i < items.Count; i++)
            {
                SellableItemWithModif item = items[i];

                DrawItem(item, items, goodRect);
                goodRect.y += 205;
            }
            Widgets.EndScrollView();
        }

        private void DrawItem(SellableItemWithModif item, List<SellableItemWithModif> itemsList, Rect rect)
        {
            bgCardColor.a = 150;
            Widgets.DrawBoxSolid(rect, bgCardColor);

            GUI.color = GUIUtils.CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            Widgets.ThingIcon(new Rect(rect.x + 8, rect.y + 18, 64, 64), item.Item);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x + 80, rect.y + 8, rect.width - 88, 25), "TraderWorker_Eisenberg_ItemLabel".Translate(item.Item.LabelNoCount, item.Item.stackCount, item.MarketValue));
            Text.Anchor = TextAnchor.UpperLeft;

            GUIUtils.DrawLineHorizontal(rect.x + 80, rect.y + 34, rect.width - 88, Color.gray);
            float y = rect.y + 36;
            Widgets.Label(new Rect(rect.x + 80, y, rect.width - 88, 120), $"TraderWorker_Eisenberg_Description".Translate(item.Item.DescriptionDetailed));

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect arrowRect = new Rect(rect.x + 10, rect.y + 165, 25, 25);
            DrawSelectArrows(item, arrowRect);
            float addX = 200;
            if (item.CountToTransfer > 0)
            {
                Widgets.Label(new Rect(rect.x + 200, rect.y + 165, 250, 25), "TraderWorker_Eisenberg_Total".Translate(item.CountToTransfer, item.CountToTransfer * item.MarketValue));
                addX = 450;
            }
            if (GUIUtils.DrawCustomButton(new Rect(rect.x + addX, rect.y + 165, 200, 25), "DarkNetButtons_Buy".Translate(), item.CountToTransfer > 0 ? Color.white : Color.gray))
            {
                if (item.CountToTransfer == 0)
                    return;

                if (DarkNetPriceUtils.BuyAndDropItem(item, item.CountToTransfer, Find.AnyPlayerHomeMap))
                {
                    if (item.Item == null)
                        itemsList.Remove(item);

                    if (item.Item != null)
                    {
                        if (item.CountToTransfer > item.Item.stackCount)
                            item.AddToTransfer(item.Item.stackCount);
                    }

                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, "TraderWorker_RogerEdmonson_FullDesc".Translate(item.Item.LabelNoCount, item.Item.DescriptionFlavor, item.MarketValue));
            }
        }


        private void DrawSelectArrows(SellableItemWithModif item, Rect rect)
        {
            if (GUIUtils.DrawCustomButton(rect, "-1", Color.white))
            {
                item.AddToTransfer(-1);
            }
            rect.x += 27;
            if (GUIUtils.DrawCustomButton(rect, "-5", Color.white))
            {
                item.AddToTransfer(-5);
            }
            rect.x += 28;
            Rect rect2 = rect;
            rect2.width = 70;
            Widgets.TextFieldNumeric(rect2, ref item.CountToTransfer, ref item.EditBuffer, 0, item.Item.stackCount);
            rect.x += 77;
            if (GUIUtils.DrawCustomButton(rect, "+1", Color.white))
            {
                item.AddToTransfer(1);
            }
            rect.x += 27;
            if (GUIUtils.DrawCustomButton(rect, "+5", Color.white))
            {
                item.AddToTransfer(5);
            }
        }
    }
}
