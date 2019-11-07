using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Traders
{
    [StaticConstructorOnStartup]
    public class Eisenberg_OrderWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(600, 600);

        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);

        private enum Tab
        {
            OneTime,
            Supplies
        }

        private Tab tab;
        private static List<TabRecord> tabsList = new List<TabRecord>();

        private TraderWorker_Eisenberg trader;

        private ThingDef selectedItem;
        private List<ThingDef> itemsToOrder;

        private int count;
        private string countBuff;

        private float discount;

        private int delay => (int)Mathf.Clamp(count * GetDelayMultiplier(selectedItem.techLevel) / trader.ArriveTime, 1, 100000);

        private float totalPrice => ((int)((selectedItem.BaseMarketValue * trader.Character.Greed) * trader.GetPriceModificatorByTechLevel(selectedItem.techLevel)) * count + (delay * 50)) * 1.1f;
        private int totalPriceFinalizaed => (int)(totalPrice - (totalPrice * discount / 100));

        private int prepayment => (int)(totalPriceFinalizaed * 0.4f);

        public Eisenberg_OrderWindow(TraderWorker_Eisenberg trader)
        {
            this.trader = trader;
            doCloseX = true;

            itemsToOrder = new List<ThingDef>();
            foreach (var setting in trader.Comp.Props.DrugStockSettings)
            {
                foreach(var good in setting.Goods)
                {
                    foreach(var thingDef in good.ThingFilter.AllowedThingDefs)
                    {
                        itemsToOrder.Add(thingDef);
                    }
                }
            }
            selectedItem = itemsToOrder.FirstOrDefault();

            discount = trader.discount;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord("Eisenberg_OrderWindow_OneTime".Translate(), delegate
            {
                tab = Tab.OneTime;
            }, tab == Tab.OneTime));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 550);
            tabsList.Clear();

            rect2.y += 65;

            switch (tab)
            {
                case Tab.OneTime:
                    DrawOneTimeWindow(rect2);
                    break;
            }
        }

        private void DrawHeader(string info)
        {
            Rect labelRect = new Rect(440, 38, 100, 24);
            Widgets.Label(labelRect, "Eisenberg_OrderWindow_InfoLabel".Translate());
            Rect textureRect = new Rect(530, 37, 24, 24);
            GUI.DrawTexture(textureRect, Info);
            TooltipHandler.TipRegion(new Rect(440, 37, 170, 24), info);
        }

        private void DrawOneTimeWindow(Rect rect)
        {
            rect.x += 10;

            DrawHeader("Eisenberg_OrderWindow_OneTimeInfo".Translate());

            Rect labelRect = new Rect(rect.x, rect.y, 120, 25);
            Widgets.Label(labelRect, "Eisenberg_OrderWindow_OneTimeItem".Translate());
            Rect itemButtonRect = new Rect(rect.x + 125, rect.y, 390, 25);
            Text.Anchor = TextAnchor.MiddleCenter;
            if(GUIUtils.DrawCustomButton(itemButtonRect, selectedItem.LabelCap, Color.white))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach(var item in itemsToOrder)
                {
                    options.Add(new FloatMenuOption($"{item.LabelCap} - {"UnitPriceDarkNet".Translate((int)((item.BaseMarketValue * trader.Character.Greed) * trader.GetPriceModificatorByTechLevel(item.techLevel)))}", delegate
                    {
                        selectedItem = item;
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }
            Text.Anchor = TextAnchor.UpperLeft;

            rect.y += 30;
            Rect countLabelRect = new Rect(rect.x, rect.y, 120, 25);
            Widgets.Label(countLabelRect, "Eisenberg_OrderWindow_OneTimeItemCount".Translate());
            Rect countnRect = new Rect(rect.x + 125, rect.y, 390, 25);
            Widgets.TextFieldNumeric(countnRect, ref count, ref countBuff, 0, 100000);

            rect.y += 32;

            Rect totalRect = new Rect(rect.x, rect.y, 564, 430);
            Widgets.Label(totalRect, "Eisenberg_OrderWindow_OneTimeTotalInfo".Translate(selectedItem.LabelCap, count, selectedItem.BaseMarketValue * trader.GetPriceModificatorByTechLevel(selectedItem.techLevel), selectedItem.BaseMarketValue * count, discount.ToString("f2"), delay, totalPriceFinalizaed));

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect bottomButton = new Rect(rect.x, rect.height - 40, 544, 25);
            if (GUIUtils.DrawCustomButton(bottomButton, "Eisenberg_OrderWindow_OneTimeDoOrder".Translate(), trader.Order == null ? Color.white : Color.gray))
            {
                if (trader.Order != null)
                {
                    Messages.Message("Eisenberg_OrderWindow_Only1Order".Translate(), MessageTypeDefOf.NeutralEvent, false);
                }
                else
                {
                    if (TakePrePayment(prepayment))
                    {
                        MakeOrder(totalPriceFinalizaed, delay, selectedItem, count);
                    }
                }
            }
            if (trader.Order != null)
            {
                if (GUIUtils.DrawCustomButton(new Rect(rect.x, rect.height, 544, 25), "DarkNetButtons_CancelOrder".Translate(), Color.white))
                {
                    trader.DeclineOrder();
                    Close();
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void MakeOrder(float price, int delay, ThingDef item, int count)
        {
            price -= prepayment;

            Order_SimpleItem order_SimpleItem = new Order_SimpleItem(1f, price, delay, item, count);
            trader.Order = order_SimpleItem;

            Find.LetterStack.ReceiveLetter("Eisenberg_OrderWindow_OneTimeDoOrder_Title".Translate(), "Eisenberg_OrderWindow_OneTimeDoOrder_Desc".Translate(selectedItem.LabelCap, count, delay), LetterDefOf.PositiveEvent);
        }

        private bool TakePrePayment(int prepayment)
        {
            return DarkNetPriceUtils.TakeSilverFromPlayer(prepayment, Find.AnyPlayerHomeMap);
        }

        private float GetDelayMultiplier(TechLevel level)
        {
            switch (level)
            {
                case TechLevel.Undefined:
                    return 1f;
                case TechLevel.Animal:
                    return 0.05f;
                case TechLevel.Neolithic:
                    return 0.05f;
                case TechLevel.Medieval:
                    return 0.08f;
                case TechLevel.Industrial:
                    return 0.1f;
                case TechLevel.Spacer:
                    return 0.3f;
                case TechLevel.Ultra:
                    return 0.5f;
                default:
                    return 1f;
            }
        }

        private float GetDelayMultiplierForSupply(TechLevel level)
        {
            switch (level)
            {
                case TechLevel.Undefined:
                    return 1f;
                case TechLevel.Animal:
                    return 0.1f;
                case TechLevel.Neolithic:
                    return 0.1f;
                case TechLevel.Medieval:
                    return 0.15f;
                case TechLevel.Industrial:
                    return 0.2f;
                case TechLevel.Spacer:
                    return 1.3f;
                case TechLevel.Ultra:
                    return 1.8f;
                default:
                    return 1f;
            }
        }
    }
}
