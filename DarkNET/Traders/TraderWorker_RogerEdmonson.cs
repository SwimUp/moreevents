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
    public enum OrderBodypartGroup
    {
        Natural,
        Simple,
        Bionic,
        Improved,
        Archotech,
        Implant
    }

    public class TraderWorker_RogerEdmonson : DarkNetTrader
    {
        private class OrderPage : Page
        {
            private Order order;
            private TraderWorker_RogerEdmonson trader;

            public override Vector2 InitialSize => new Vector2(580, 197);

            public OrderPage(Order order, TraderWorker_RogerEdmonson trader)
            {
                this.order = order;
                this.trader = trader;
                doCloseX = true;
            }

            public override void DoWindowContents(Rect inRect)
            {
                DrawOrderCard(inRect, order);
            }

            public void DrawOrderCard(Rect rect, Order item)
            {
                trader.bgCardColor.a = 150;
                Widgets.DrawBoxSolid(rect, trader.bgCardColor);

                GUI.color = GUIUtils.CommBorderColor;
                Widgets.DrawBox(rect);
                GUI.color = Color.white;

                Widgets.ThingIcon(new Rect(rect.x + 8, rect.y + 18, 64, 64), item.OrderedItem);

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(rect.x + 80, rect.y + 8, rect.width - 88, 25), item.OrderedItem.Label);
                Text.Anchor = TextAnchor.UpperLeft;

                GUIUtils.DrawLineHorizontal(rect.x + 80, rect.y + 34, rect.width - 88, Color.gray);
                float y = rect.y + 36;
                Widgets.Label(new Rect(rect.x + 80, y, rect.width - 88, 25), $"DarkNetModificator_Price".Translate(item.Price));

                Text.Anchor = TextAnchor.MiddleCenter;
                if (GUIUtils.DrawCustomButton(new Rect(rect.x, rect.y + 105, rect.width, 25), "DarkNetButtons_Buy".Translate(), Color.white))
                {
                    if (trader.AcceptOrder())
                    {
                        Close();
                        Text.Anchor = TextAnchor.UpperLeft;
                        return;
                    }
                }
                if (GUIUtils.DrawCustomButton(new Rect(rect.x, rect.y + 132, rect.width, 25), "DarkNetButtons_CancelOrder".Translate(), Color.white))
                {
                    trader.DeclineOrder();
                    Close();
                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
                Text.Anchor = TextAnchor.UpperLeft;

                if (Mouse.IsOver(rect))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("TraderWorker_RogerEdmonson_FullDesc".Translate(item.OrderedItem.Label, item.OrderedItem.DescriptionFlavor, item.Price));

                    TooltipHandler.TipRegion(rect, builder.ToString());
                }
            }
        }

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

        private float specialGoodMarketValue => 6000;

        private int lastRaidsEnemy = 0;

        public List<SellableItemWithModif> StockForReading => stock;

        private List<SellableItemWithModif> stock;

        private SellableItemWithModif goodOfTheWeek;

        private Vector2 slider = Vector2.zero;

        protected Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        private ThingFilter specialGoodsFilter => RogerComp.Props.specialGoodsFilter;

        public List<OrderItem> OrderBodyparts => RogerComp.Props.OrderBodyparts;

        public Order Order;

        private float raidMultiplier = 1.5f;

        public DarkNetComp_RogerEdmonson RogerComp
        {
            get
            {
                if (rogerComp == null)
                {
                    rogerComp = TryGetComp<DarkNetComp_RogerEdmonson>();
                }

                return rogerComp;
            }
        }

        private DarkNetComp_RogerEdmonson rogerComp;

        public override void FirstInit()
        {
            base.FirstInit();

            if(specialGoodsFilter != null)
            {
                specialGoodsFilter.ResolveReferences();
            }
        }

        public override void DrawTraderShop(Rect rect)
        {
            Rect imageRect = new Rect(700, rect.y, 400, rect.height);
            GUI.DrawTexture(imageRect, def.FullTexture);

            Rect specialGoodsRect = new Rect(rect.x + 10, rect.y + 2, 580, 30);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(specialGoodsRect, "RogerEdmonson_SpecialGoods".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            specialGoodsRect.y += 25;
            specialGoodsRect.height = 130;
            if (goodOfTheWeek != null)
            {
                DrawCustomItemCard(specialGoodsRect, goodOfTheWeek);
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;
                Widgets.Label(specialGoodsRect, "TraderWorker_RogerEdmonson_NoSpecialGood".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
            GUIUtils.DrawLineHorizontal(rect.x, 202, 600, Color.gray);

            Rect goodsRect = new Rect(rect.x + 10, 210, 600, 420);
            Rect goodRect = new Rect(0, 0, 580, 130);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, stock.Count * 140);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for (int i = 0; i < stock.Count; i++)
            {
                var item = stock[i];

                DrawCustomItemCard(goodRect, item);

                goodRect.y += 140;
            }
            Widgets.EndScrollView();

            GUIUtils.DrawLineHorizontal(rect.x, 635, 610, Color.gray);

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect bottomButtonsRect = new Rect(rect.x + 10, 645, 600, 25);
            if(GUIUtils.DrawCustomButton(bottomButtonsRect, "TraderWorker_RogerEdmonson_MakeOrder".Translate(), Color.white))
            {
                Find.WindowStack.Add(new RogerEdmonson_OrderWindow(this));
            }
            bottomButtonsRect.y += 30;
            Color bColor = Color.gray;
            if (Order != null && Order.Finish)
                bColor = Color.white;

            if (GUIUtils.DrawCustomButton(bottomButtonsRect, "TraderWorker_RogerEdmonson_GetOrder".Translate(), bColor))
            {
                if (Order != null && Order.Success)
                {
                    Find.WindowStack.Add(new OrderPage(Order, this));
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }
        public void DrawCustomItemCard(Rect rect, SellableItemWithModif item)
        {
            bgCardColor.a = 150;
            Widgets.DrawBoxSolid(rect, bgCardColor);

            GUI.color = GUIUtils.CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            Widgets.ThingIcon(new Rect(rect.x + 8, rect.y + 18, 64, 64), item.Item);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x + 80, rect.y + 8, rect.width - 88, 25), item.Item.Label);
            Text.Anchor = TextAnchor.UpperLeft;

            GUIUtils.DrawLineHorizontal(rect.x + 80, rect.y + 34, rect.width - 88, Color.gray);
            float y = rect.y + 36;
            Widgets.Label(new Rect(rect.x + 80, y, rect.width - 88, 25), $"DarkNetModificator_Price".Translate(item.MarketValue));
            y += 25;
            if (item.Modificator != null)
            {
                Widgets.Label(new Rect(rect.x + 80, y, rect.width - 88, 40), item.Modificator.LabelCap);
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            if (GUIUtils.DrawCustomButton(new Rect(rect.x, rect.y + 105, rect.width, 25), "DarkNetButtons_Buy".Translate(), Color.white))
            {
                if (DarkNetPriceUtils.BuyAndDropItem(item, Find.AnyPlayerHomeMap))
                {
                    if (stock.Contains(item))
                        stock.Remove(item);
                    else goodOfTheWeek = null;

                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(rect))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("TraderWorker_RogerEdmonson_FullDesc".Translate(item.Item.Label, item.Item.DescriptionFlavor, item.MarketValue));
                if (item.Modificator != null)
                {
                    builder.Append("TraderWorker_RogerEdmonson_Modificator".Translate(item.Modificator.LabelCap, item.Modificator.description, item.Modificator.PriceModficator));
                }

                TooltipHandler.TipRegion(rect, builder.ToString());
            }
        }

        public void DeclineOrder()
        {
            if(Order.OrderedItem != null)
            {
                Order.OrderedItem.Destroy();
            }

            Order = null;
            Find.LetterStack.ReceiveLetter("TraderWorker_RogerEdmonson_CancelOrderTitle".Translate(), "TraderWorker_RogerEdmonson_CancelOrderDesc".Translate(), LetterDefOf.NegativeEvent);
            SendCancelOrderRaid();
        }

        public bool AcceptOrder()
        {
            if (DarkNetPriceUtils.BuyAndDropItem(Order.OrderedItem, (int)Order.Price, Find.AnyPlayerHomeMap, true))
            {
                Order = null;
                return true;
            }

            return false;
        }

        public override void OnDayPassed()
        {
            base.OnDayPassed();

            if (Online)
            {
                if (Rand.Chance(0.13f))
                {
                    RandomBuy();
                }
            }
        }

        public override void TraderGone()
        {
            base.TraderGone();

            if(Order != null)
            {
                if(Order.Finish && Order.Success)
                {
                    DeclineOrder();
                }
            }
        }

        private void SendCancelOrderRaid()
        {
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.target = Find.AnyPlayerHomeMap;
            incidentParms.faction = Find.FactionManager.RandomEnemyFaction();
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(incidentParms.target) * raidMultiplier;
            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + Rand.Range(2, 3) * 60000, incidentParms);

            raidMultiplier += 0.4f;
        }

        private void RandomBuy()
        {
            if (stock.Count == 0)
                return;

            int itemPos = Rand.Range(0, stock.Count);

            SellableItemWithModif item = stock[itemPos];

            item.Item.Destroy();
            stock.Remove(item);
        }


        public override void Arrive()
        {
            RegenerateStock();

            if (Order != null)
            {
                Order.TraderArrive(this);
            }
        }

        public virtual void RegenerateStock()
        {
            TryDestroyStock();

            int raidsCount = Find.StoryWatcher.statsRecord.numRaidsEnemy - lastRaidsEnemy;
            lastRaidsEnemy = raidsCount;

            int itemsCount = (int)itemsCountPerRaidCurve.Evaluate(raidsCount);
            float valueRange = startMarketValue + (marketValueMultiplierPerMapEvent * raidsCount);

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();

            ThingSetMakerParams parms = default;
            parms.totalMarketValueRange = new FloatRange(valueRange, valueRange);
            parms.countRange = new IntRange(itemsCount, itemsCount);

            parms.filter = DarkNetPriceUtils.GetThingFilter(def.AvaliableGoods);

            maker.fixedParams = parms;

            var items = maker.Generate();
            stock = new List<SellableItemWithModif>();

            foreach(var item in items)
            {
                int itemValue = (int)(item.MarketValue * Character.Greed);

                if (PriceModificatorUtils.TryGetPriceModificator(item, def, out PriceModificatorDef modificator))
                {
                    itemValue = (int)(itemValue * modificator.PriceModficator);
                    DarkNetPriceUtils.FinalizeItem(item, modificator);
                }

                MinifiedThing minifiedThing = item as MinifiedThing;
                if (minifiedThing != null)
                {
                    var quality = minifiedThing.InnerThing.TryGetComp<CompQuality>();
                    if (quality != null)
                    {
                        quality.SetQuality(QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Colony);
                        itemValue = (int)(itemValue * GetPriceMultiplierForQuality(quality.Quality));
                    }
                }
                else
                {
                    var quality = item.TryGetComp<CompQuality>();
                    if(quality != null)
                    {
                        quality.SetQuality(QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Colony);
                        itemValue = (int)(itemValue * GetPriceMultiplierForQuality(quality.Quality));
                    }
                }

                stock.Add(new SellableItemWithModif(item, itemValue, modificator));
            }

            if (raidsCount >= 10 && Rand.Chance(0.3f))
            {
                parms.totalMarketValueRange = new FloatRange(specialGoodMarketValue, specialGoodMarketValue);
                parms.countRange = new IntRange(1, 1);
                parms.filter = specialGoodsFilter;
                maker.fixedParams = parms;
                Thing generalGood = maker.Generate().FirstOrDefault();
                if (generalGood != null)
                {
                    goodOfTheWeek = new SellableItemWithModif(generalGood, (int)(generalGood.MarketValue * Character.Greed), null);
                }
            }
        }

        public void TryDestroyStock()
        {
            if (stock == null)
            {
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
            stock = null;

            if(goodOfTheWeek != null)
            {
                if (goodOfTheWeek.Item != null)
                {
                    if (!(goodOfTheWeek.Item is Pawn) && !goodOfTheWeek.Item.Destroyed)
                    {
                        goodOfTheWeek.Item.Destroy();
                    }
                }
                goodOfTheWeek = null;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastRaidsEnemy, "lastRaidsEnemy");
            Scribe_Collections.Look(ref stock, "stock", LookMode.Deep);
            Scribe_Values.Look(ref raidMultiplier, "raidMultiplier");
            Scribe_Deep.Look(ref goodOfTheWeek, "goodOfTheWeek");
            Scribe_Deep.Look(ref Order, "Order");
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
        public float GetPriceMultiplier(OrderBodypartGroup group)
        {
            switch(group)
            {
                case OrderBodypartGroup.Natural:
                    {
                        return 1.4f;
                    }
                case OrderBodypartGroup.Simple:
                    {
                        return 1.25f;
                    }
                case OrderBodypartGroup.Bionic:
                    {
                        return 6f;
                    }
                case OrderBodypartGroup.Archotech:
                    {
                        return 14f;
                    }
                case OrderBodypartGroup.Improved:
                    {
                        return 5f;
                    }
                case OrderBodypartGroup.Implant:
                    {
                        return 12f;
                    }
            }

            return 1f;
        }

        public float GetChanceMultiplier(OrderBodypartGroup group)
        {
            switch (group)
            {
                case OrderBodypartGroup.Natural:
                    {
                        return 1.0f;
                    }
                case OrderBodypartGroup.Simple:
                    {
                        return 0.95f;
                    }
                case OrderBodypartGroup.Bionic:
                    {
                        return 0.80f;
                    }
                case OrderBodypartGroup.Archotech:
                    {
                        return 0.65f;
                    }
                case OrderBodypartGroup.Improved:
                    {
                        return 0.85f;
                    }
                case OrderBodypartGroup.Implant:
                    {
                        return 0.3f;
                    }
            }

            return 1f;
        }
    }
}
