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
    public class CategoryItem<T> : IExposable
    {
        public T Tab;

        public List<SellableItemWithModif> Items;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Tab, "Tab");
            Scribe_Collections.Look(ref Items, "Items", LookMode.Deep);
        }
    }

    public class TraderWorker_JohnCarver : DarkNetTrader
    {
        private Vector2 slider = Vector2.zero;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        public enum Tab
        {
            HardMetals,
            Experimental,
            Precious,
            Weapons,
            Artifacts
        }

        private static Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private List<CategoryItem<Tab>> stock;
        public List<CategoryItem<Tab>> Stock => stock;

        public override int ArriveTime => 8;

        public override int OnlineTime => 3;

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
            stock = new List<CategoryItem<Tab>>();

            foreach (Tab cat in Enum.GetValues(typeof(Tab)))
            {
                CategoryItem<Tab> catItem = new CategoryItem<Tab>
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
                CategoryItemSetting<Tab> settings = Comp.Props.CategoryItemSettings.First(x => x.Tab == cat.Tab);

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
                    if (!DarkNetPriceUtils.TryMerge(item, cat.Items))
                    {
                        int marketValue = (int)((item.MarketValue * Character.Greed) * settings.PriceMultiplier);

                        var quality = item.TryGetComp<CompQuality>();
                        if (quality != null)
                        {
                            quality.SetQuality(QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Colony);
                            marketValue = (int)(marketValue * GetPriceMultiplierForQuality(quality.Quality));
                        }

                        SellableItemWithModif newItem = new SellableItemWithModif(item, marketValue, null);

                        cat.Items.Add(newItem);
                    }
                }
            }
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
            tabsList.Add(new TabRecord("TraderWorker_JohnCarver_Tab_Artifacts".Translate(), delegate
            {
                tab = Tab.Artifacts;
            }, tab == Tab.Artifacts));
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

                GUIUtils.DrawItemCard(item, items, goodRect);
                goodRect.y += 205;
            }
            Widgets.EndScrollView();
        }

        public float GetPriceMultiplierForQuality(QualityCategory qualityCategory)
        {
            switch (qualityCategory)
            {
                case QualityCategory.Awful:
                    {
                        return 0.8f;
                    }
                case QualityCategory.Poor:
                    {
                        return 0.9f;
                    }
                case QualityCategory.Normal:
                    {
                        return 1f;
                    }
                case QualityCategory.Good:
                    {
                        return 1.1f;
                    }
                case QualityCategory.Excellent:
                    {
                        return 1.2f;
                    }
                case QualityCategory.Masterwork:
                    {
                        return 1.3f;
                    }
                case QualityCategory.Legendary:
                    {
                        return 1.5f;
                    }
            }

            return 1f;
        }
    }
}
