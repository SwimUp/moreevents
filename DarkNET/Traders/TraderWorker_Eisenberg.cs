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
    public class TraderWorker_Eisenberg : DarkNetTrader
    {
        private Vector2 slider = Vector2.zero;

        private class CategoryDrug : IExposable
        {
            public Tab Tab;

            public List<SellableItemWithModif> Items;

            public void ExposeData()
            {
                Scribe_Values.Look(ref Tab, "Tab");
                Scribe_Collections.Look(ref Items, "Items", LookMode.Deep);
            }
        }

        public override int ArriveTime => 2;

        public override int OnlineTime => 1;

        public enum Tab
        {
            Alcohol,
            EasyDrugs,
            HeavyDrugs,
            Experimental
        }

        private static Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private List<CategoryDrug> drugs;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        public float Reputation;
        public float discount => 0.25f * Reputation;

        private bool needRecalculate = false;

        public DarkNetComp_Eisenberg Comp
        {
            get
            {
                if (сomp == null)
                {
                    сomp = TryGetComp<DarkNetComp_Eisenberg>();
                }

                return сomp;
            }
        }

        private DarkNetComp_Eisenberg сomp;

        public override void FirstInit()
        {
            base.FirstInit();

            Inititialize();
        }

        private void Inititialize()
        {
            drugs = new List<CategoryDrug>();

            foreach(Tab cat in Enum.GetValues(typeof(Tab)))
            {
                drugs.Add(new CategoryDrug
                {
                    Tab = cat,
                    Items = new List<SellableItemWithModif>()
                });
            }
        }

        public override void DrawTraderShop(Rect rect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = rect;
            rect2.yMin += 35;

            Widgets.DrawMenuSection(rect2);
            tabsList.Clear();
            tabsList.Add(new TabRecord("TraderWorker_Eisenberg_Tab_Alcohol".Translate(), delegate
            {
                tab = Tab.Alcohol;
            }, tab == Tab.Alcohol));
            tabsList.Add(new TabRecord("TraderWorker_Eisenberg_Tab_EasyDrugs".Translate(), delegate
            {
                tab = Tab.EasyDrugs;
            }, tab == Tab.EasyDrugs));
            tabsList.Add(new TabRecord("TraderWorker_Eisenberg_Tab_HeavyDrugs".Translate(), delegate
            {
                tab = Tab.HeavyDrugs;
            }, tab == Tab.HeavyDrugs));
            tabsList.Add(new TabRecord("TraderWorker_Eisenberg_Tab_Experimental".Translate(), delegate
            {
                tab = Tab.Experimental;
            }, tab == Tab.Experimental));
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 245);

            DrawItems(tab, rect2);

            Text.Font = GameFont.Medium;
            Rect additionalInfoRect = new Rect(260, 0, 400, 30);
            Widgets.Label(additionalInfoRect, "TraderWorker_Eisenberg_Tab_Reputation".Translate(Mathf.RoundToInt(Reputation)));
            TooltipHandler.TipRegion(additionalInfoRect, "TraderWorker_Eisenberg_Tab_ReputationInfo");
            Text.Font = GameFont.Small;
        }

        private void DrawItems(Tab tab, Rect rect)
        {
            List<SellableItemWithModif> items = drugs.First(x => x.Tab == tab).Items;

            Rect goodsRect = new Rect(rect.x + 10, rect.y + 5, rect.width - 15, rect.height - 25);
            Rect goodRect = new Rect(0, 0, goodsRect.width - 25, 200);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, items.Count * 205);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for(int i = 0; i < items.Count; i++)
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

                if(DarkNetPriceUtils.BuyAndDropItem(item, item.CountToTransfer, Find.AnyPlayerHomeMap))
                {
                    if(item.Item == null)
                        itemsList.Remove(item);

                    Reputation += (item.CountToTransfer * item.MarketValue) * 0.004f;
                    needRecalculate = true;

                    if (item.CountToTransfer > item.Item.stackCount)
                        item.AddToTransfer(item.Item.stackCount);

                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
            }
            addX += 205;
            if (GUIUtils.DrawCustomButton(new Rect(rect.x + addX, rect.y + 165, 200, 25), "DarkNetButtons_GoPriceDown".Translate(), item.CountToTransfer > 0 ? Color.white : Color.gray))
            {
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
                item.AddToTransfer(-10);
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
                item.AddToTransfer(10);
            }
        }

        public override void Arrive()
        {
            base.Arrive();

            RegenerateStock();
        }

        public override void WindowOpen()
        {
            base.WindowOpen();

            if(needRecalculate)
                RecalculatePrices();
        }

        private void RecalculatePrices()
        {
            float tmpDiscount = discount;

            foreach(var cat in drugs)
            {
                foreach(var item in cat.Items)
                {
                    item.MarketValue = (int)((item.MarketValue * Character.Greed) * GetPriceModificatorByTechLevel(item.Item.def.techLevel));
                    item.MarketValue -= (int)(item.MarketValue * tmpDiscount);
                }
            }

            needRecalculate = false;
        }

        public void RegenerateStock()
        {
            TryDestroyStock();

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms = default;
            parms.filter = DarkNetPriceUtils.GetThingFilter(def.AvaliableGoods);

            float tmpDiscount = discount;

            foreach (var cat in drugs)
            {
                DrugSettings settings = Comp.Props.DrugStockSettings.First(x => x.Tab == Tab.Alcohol);

                int itemsCount = settings.CountRange.RandomInRange;
                float valueRange = settings.ValueRange.RandomInRange * itemsCount;

                parms.totalMarketValueRange = new FloatRange(valueRange, valueRange);
                parms.countRange = new IntRange(itemsCount, itemsCount);

                maker.fixedParams = parms;

                var items = maker.Generate();

                foreach (var item in items)
                {
                    if (!TryMerge(item, cat.Items))
                    {
                        int marketValue = (int)((item.MarketValue * Character.Greed) * GetPriceModificatorByTechLevel(item.def.techLevel));
                        marketValue -= (int)(marketValue * tmpDiscount);

                        SellableItemWithModif newItem = new SellableItemWithModif(item, marketValue, null);

                        cat.Items.Add(newItem);
                    }
                }
            }
        }

        private bool TryMerge(Thing item, List<SellableItemWithModif> stock)
        {
            for(int i = 0; i < stock.Count; i++)
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
            if (drugs == null)
            {
                Inititialize();
                return;
            }

            foreach(var drug in drugs)
            {
                for (int num2 = drug.Items.Count - 1; num2 >= 0; num2--)
                {
                    SellableItemWithModif item = drug.Items[num2];

                    if (item.Item != null)
                    {
                        Thing thing = item.Item;

                        if (!(thing is Pawn) && !thing.Destroyed)
                        {
                            thing.Destroy();
                        }
                    }
                }

                drug.Items = null;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref drugs, "drugs", LookMode.Deep);
            Scribe_Values.Look(ref Reputation, "Reputation");
        }

        public float GetPriceModificatorByTechLevel(TechLevel level)
        {
            switch(level)
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
