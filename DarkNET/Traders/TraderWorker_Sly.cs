using DarkNET.Sly;
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

        public override int ArriveTime => 7;

        public override int OnlineTime => 2;

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

        public List<SlyService> Services
        {
            get
            {
                if (services == null)
                {
                    InitServices();
                }

                return services;
            }
        }

        private List<SlyService> services;

        private Type[] slyServices = new Type[]
        {
            typeof(SlyService_RaidHelp),
            typeof(SlyService_HumanitarianHelp),
            typeof(SlyService_ResoucesHelp)
        };

        public override void FirstInit()
        {
            base.FirstInit();

            Inititialize();
        }

        public override bool TryGetGoods(List<Thing> goods)
        {
            if (stock == null)
                return false;

            foreach(var item in stock)
            {
                if(item.Item != null)
                {
                    goods.Add(item.Item);
                }
            }

            return true;
        }

        public override void OnDayPassed()
        {
            base.OnDayPassed();

            if (Services != null)
            {
                foreach (var service in Services)
                {
                    service.SlyDayPassed(this);
                }
            }
        }

        public override void TraderGone()
        {
            base.TraderGone();

            if (Services != null)
            {
                foreach (var service in Services)
                {
                    service.SlyGone(this);
                }
            }
        }

        private void InitServices()
        {
            services = new List<SlyService>();

            foreach(var service in slyServices)
            {
                SlyService slyService = (SlyService)Activator.CreateInstance(service);

                services.Add(slyService);
            }
        }

        private void Inititialize()
        {
            stock = new List<SellableItemWithModif>();

            InitServices();
        }

        public override void Arrive()
        {
            base.Arrive();

            RegenerateStock();

            if(Services != null)
            {
                foreach(var service in Services)
                {
                    service.SlyArrival(this);
                }
            }
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
            Scribe_Collections.Look(ref services, "services", LookMode.Deep);
        }

        public override void DrawTraderShop(Rect rect, Pawn speaker)
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
            tabsList.Add(new TabRecord("TraderWorker_Sly_Tab_Service".Translate(), delegate
            {
                tab = Tab.Service;
            }, tab == Tab.Service));
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 490);

            switch (tab)
            {
                case Tab.Items:
                    {
                        DrawItems(rect2, speaker);
                        break;
                    }
                case Tab.Service:
                    {
                        DrawServices(rect2, speaker.Map);
                        break;
                    }
            }

            Text.Font = GameFont.Small;
        }

        private void DrawServices(Rect rect, Map map)
        {
            List<SellableItemWithModif> items = stock;

            Rect goodsRect = new Rect(rect.x + 10, rect.y + 5, rect.width - 15, rect.height - 25);
            Rect goodRect = new Rect(0, 0, goodsRect.width - 25, 200);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, Services.Count * 225);
            Widgets.BeginScrollView(goodsRect, ref slider, scrollVertRectFact, true);
            for (int i = 0; i < Services.Count; i++)
            {
                SlyService item = Services[i];

                DrawService(item, goodRect, map);
                goodRect.y += 205;
            }
            Widgets.EndScrollView();
        }

        private void DrawService(SlyService service, Rect rect, Map map)
        {
            bgCardColor.a = 150;
            Widgets.DrawBoxSolid(rect, bgCardColor);

            GUI.color = GUIUtils.CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x + 130, rect.y + 8, rect.width - 88, 25), service.Label);

            Text.Anchor = TextAnchor.UpperLeft;

            GUIUtils.DrawLineHorizontal(rect.x + 205, rect.y + 34, rect.width - 213, Color.gray);
            float y = rect.y + 36;
            Widgets.Label(new Rect(rect.x + 205, y, rect.width - 213, 140), service.Description);

            Text.Anchor = TextAnchor.MiddleCenter;

            bool serviceStatus = service.AvaliableRightNow(out string reason);
            Rect bRect = new Rect(rect.x + 8, rect.y + 36, 190, 25);
            foreach (var option in service.Options(map))
            {
                if (GUIUtils.DrawCustomButton(bRect, option.Label, serviceStatus ? Color.white : Color.gray))
                {
                    if(serviceStatus)
                        option.action.Invoke();
                }

                bRect.y += 32;
            }

            if(!serviceStatus)
                Widgets.Label(new Rect(rect.x + 8, rect.y + 175, rect.width - 8, 25), "DarkNetLabels_ServiceUnavaliable".Translate(reason));

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawItems(Rect rect, Pawn speaker)
        {
            List<SellableItemWithModif> items = stock;

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
