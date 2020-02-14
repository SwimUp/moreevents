using DarkNET.Quests;
using DarkNET.TraderComp;
using MoreEvents;
using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Traders
{
    public class TraderWorker_Archotech_567H : DarkNetTrader
    {
        private Vector2 slider = Vector2.zero;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        private EventSettings settings => Settings.EventsSettings["TraderWorker_Archotech_567H"];

        public enum Tab
        {
            Items,
            Armors,
            Weapons
        }

        private static Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private List<CategoryItem<Tab>> stock;
        public List<CategoryItem<Tab>> Stock => stock;

        public override int ArriveTime => arrivaTime;
        private int arrivaTime;

        public override int OnlineTime => onlineTime;
        private int onlineTime;

        public DarkNetComp_Archotech_567H Comp
        {
            get
            {
                if (сomp == null)
                {
                    сomp = TryGetComp<DarkNetComp_Archotech_567H>();
                }

                return сomp;
            }
        }

        private DarkNetComp_Archotech_567H сomp;

        public List<Quest> Quests = new List<Quest>();

        private Type[] QuestsList = new Type[]
        {
            typeof(Quest_Archotech_567H_KillAll),
            typeof(Quest_Archotech_567H_KillOponents),
            typeof(Quest_Archotech_567H_GetResources)
        };

        public TraderWorker_Archotech_567H() : base()
        {
            onlineTime = int.Parse(settings.Parameters["onlineTime"].Value);
            arrivaTime = int.Parse(settings.Parameters["arrivaTime"].Value);
        }

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

        public override bool TryGetGoods(List<Thing> goods)
        {
            if (stock == null || !stock.Any(x => x.Items.Any()))
                return false;

            foreach(var cat in stock)
            {
                if (cat.Items != null)
                {
                    foreach (var item in cat.Items)
                    {
                        if(item != null && item.Item != null)
                            goods.Add(item.Item);
                    }
                }
            }

            return true;
        }

        public override void Arrive()
        {
            base.Arrive();

            RegenerateStock();

            TryGenerateQuests();
        }

        public void TryGenerateQuests()
        {
            int questCount = Rand.RangeInclusive(1, 2);
            for (int i = 0; i < questCount; i++)
            {
                int questId = Rand.Range(0, QuestsList.Length);
                Quest quest = (Quest)Activator.CreateInstance(QuestsList[questId]);
                if (quest.TryGiveQuestTo(null, null))
                {
                    Quests.Add(quest);
                }

            }
        }

        public override void TraderGone()
        {
            if (Quests != null)
            {
                for (int i = 0; i < Quests.Count; i++)
                {
                    Quest quest = Quests[i];
                    if (quest != null)
                    {
                        if (quest.Site != null)
                        {
                            quest.Site.EndQuest(null, EndCondition.None);
                        }
                        else
                        {
                            quest.EndQuest(null, EndCondition.None);
                        }
                    }
                }
            }
            Quests.Clear();

            base.TraderGone();
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
                            quality.SetQuality((QualityCategory)Rand.RangeInclusive(4, 6), ArtGenerationContext.Colony);
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
            Scribe_Collections.Look(ref Quests, "Quests", LookMode.Reference);
        }

        public override void DrawTraderShop(Rect rect, Pawn speaker)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = rect;
            rect2.yMin += 35;

            Widgets.DrawMenuSection(rect2);
            tabsList.Clear();
            tabsList.Add(new TabRecord("TraderWorker_Archotech_567H_Tab_Items".Translate(), delegate
            {
                tab = Tab.Items;
            }, tab == Tab.Items));
            tabsList.Add(new TabRecord("TraderWorker_Archotech_567H_Tab_Weapons".Translate(), delegate
            {
                tab = Tab.Weapons;
            }, tab == Tab.Weapons));
            tabsList.Add(new TabRecord("TraderWorker_Archotech_567H_Tab_Armors".Translate(), delegate
            {
                tab = Tab.Armors;
            }, tab == Tab.Armors));
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 320);

            DrawItems(tab, rect2, speaker);

            Text.Font = GameFont.Medium;

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect additionalInfoRect = new Rect(600, 0, 210, 30);
            if(GUIUtils.DrawCustomButton(additionalInfoRect, "TraderWorker_Archotech_567H_Quests".Translate(), Color.white))
            {
                Find.WindowStack.Add(new DarkNETWindow_Quests(Quests));
            }
            TooltipHandler.TipRegion(additionalInfoRect, "TraderWorker_Archotech_567H_Quests_Info".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            Text.Font = GameFont.Small;
        }

        private void DrawItems(Tab tab, Rect rect, Pawn speaker)
        {
            List<SellableItemWithModif> items = stock.First(x => x.Tab == tab).Items;

            Rect goodsRect = new Rect(rect.x + 10, rect.y + 5, rect.width - 15, rect.height - 25);
            Rect goodRect = new Rect(0, 0, goodsRect.width - 25, 200);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, items.Count * 205);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for (int i = 0; i < items.Count; i++)
            {
                SellableItemWithModif item = items[i];

                GUIUtils.DrawItemCard(item, items, goodRect, speaker);
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
