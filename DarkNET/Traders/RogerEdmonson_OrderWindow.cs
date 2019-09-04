using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Traders
{
    public class RogerEdmonson_OrderWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(600, 600);

        private enum Tab
        {
            BodyParts,
            Arts
        }

        private Tab tab;
        private static List<TabRecord> tabsList = new List<TabRecord>();

        private TraderWorker_RogerEdmonson trader;

        private OrderBodypartGroup group;
        private int delay;
        private string delayBuff;

        private int baseValue => 300;

        private int totalValue = 0;
        private float chance = 0f;

        private int prepayment;

        private ThingDef artStuff;

        private float baseChance => 45f;

        private ThingDef artType;

        public RogerEdmonson_OrderWindow(TraderWorker_RogerEdmonson trader)
        {
            this.trader = trader;
            delay = 0;
            doCloseX = true;

            artStuff = DefDatabase<ThingDef>.AllDefs.Where(x => x.IsStuff).FirstOrDefault();
            artType = DefDatabase<ThingDef>.AllDefs.Where(x => x.IsArt).FirstOrDefault();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord("RogerEdmonson_OrderWindow_BodyParts".Translate(), delegate
            {
                tab = Tab.BodyParts;
            }, tab == Tab.BodyParts));
            tabsList.Add(new TabRecord("RogerEdmonson_OrderWindow_Arts".Translate(), delegate
            {
                tab = Tab.Arts;
            }, tab == Tab.Arts));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 300);
            tabsList.Clear();

            rect2.y += 32;

            switch (tab)
            {
                case Tab.BodyParts:
                    DrawBodyPartsOders(rect2);
                    break;
                case Tab.Arts:
                    DrawArtsOrders(rect2);
                    break;
            }
        }

        private void DrawBodyPartsOders(Rect rect)
        {
            CalculateBodyPartValues();

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect buttonRect = new Rect(rect.x + 10, rect.y, rect.width - 20, 25);
            if(GUIUtils.DrawCustomButton(buttonRect, "RogerEdmonson_OrderWindow_SelectGroup".Translate($"{group}_group".Translate()), Color.white))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach(OrderBodypartGroup group in Enum.GetValues(typeof(OrderBodypartGroup)))
                {
                    options.Add(new FloatMenuOption($"{group}_group".Translate(), delegate {
                        this.group = group;
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }
            Text.Anchor = TextAnchor.UpperLeft;

            Rect intRect = new Rect(rect.x + 10, rect.y + 32, 250, 25);
            Widgets.Label(intRect, "RogerEdmonson_OrderWindow_Range".Translate());
            intRect.x += 255;
            intRect.width = rect.width - 275;
            Widgets.TextFieldNumeric(intRect, ref delay, ref delayBuff, 1);

            Rect fullLabel = new Rect(rect.x + 10, rect.y + 60, rect.width - 10, 300);
            Widgets.Label(fullLabel, "RogerEdmonson_OrderWindow_Full".Translate($"{group}_desc".Translate(), totalValue, chance, prepayment));
            TooltipHandler.TipRegion(fullLabel, "RogerEdmonson_OrderWindow_Full2".Translate($"{group}_group".Translate(), baseValue, trader.GetPriceMultiplier(group), delay, trader.ArriveTime, totalValue, prepayment, chance, baseChance, trader.GetChanceMultiplier(group)));

            Text.Anchor = TextAnchor.MiddleCenter;
            if (GUIUtils.DrawCustomButton(new Rect(rect.x + 10, rect.height - 40, rect.width - 20, 30), "RogerEdmonson_OrderWindow_CreateOrder".Translate(), trader.Order == null ? Color.white : Color.gray))
            {
                if (trader.Order != null)
                {
                    Messages.Message("RogerEdmonson_OrderWindow_Only1Order".Translate(), MessageTypeDefOf.NeutralEvent, false);
                }
                else
                {
                    if (TakePrePayment(prepayment))
                    {
                        MakeBodyPartOrder(group, chance, totalValue, delay);
                    }
                }
            }
            if(trader.Order != null)
            {
                if (GUIUtils.DrawCustomButton(new Rect(rect.x + 10, rect.height, rect.width - 20, 30), "DarkNetButtons_CancelOrder".Translate(), Color.white))
                {
                    trader.DeclineOrder();
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void MakeBodyPartOrder(OrderBodypartGroup group, float chance, float price, int delay)
        {
            price -= prepayment;

            Order_BodyPart_RogerEdmonson order = new Order_BodyPart_RogerEdmonson(chance / 100, price, delay, group);
            trader.Order = order;

            Find.LetterStack.ReceiveLetter("MakeBodyPartOrder_Title".Translate(), "MakeBodyPartOrder_Desc".Translate($"{group}_group".Translate(), delay), LetterDefOf.PositiveEvent);
        }

        private void MakeArtOrder(float chance, float price, int delay, ThingDef artType, ThingDef artStuff)
        {
            price -= prepayment;

            Order_Art_RogerEdmonson order = new Order_Art_RogerEdmonson(chance / 100, price, delay, artType, artStuff);
            trader.Order = order;

            Find.LetterStack.ReceiveLetter("MakeBodyPartOrder_Title".Translate(), "MakeBodyPartOrder_Desc".Translate($"{group}_group".Translate(), delay), LetterDefOf.PositiveEvent);
        }

        private bool TakePrePayment(int prepayment)
        {
            return DarkNetPriceUtils.TakeSilverFromPlayer(prepayment, Find.AnyPlayerHomeMap);
        }

        private void CalculateBodyPartValues()
        {
            totalValue = (int)(baseValue * trader.GetPriceMultiplier(group) + (100 * delay));
            chance = Mathf.Clamp((baseChance + (25f * delay)) * trader.GetChanceMultiplier(group), 0, 100);
            prepayment = (int)(totalValue * 0.4f);
        }

        private void CalculateArtValues()
        {
            totalValue = (int)(baseValue * ((artType.costStuffCount * artStuff.BaseMarketValue) * 0.03f));
            chance = Mathf.Round(Mathf.Clamp(((baseChance / (artType.costStuffCount * 0.006f)) * delay) - (artStuff.BaseMarketValue * 1.2f), 0, 100));
            prepayment = (int)(totalValue * 0.4f);
        }

        private void DrawArtsOrders(Rect rect)
        {
            CalculateArtValues();

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect buttonRect = new Rect(rect.x + 10, rect.y, rect.width - 20, 25);
            if (GUIUtils.DrawCustomButton(buttonRect, "RogerEdmonson_OrderWindow_SelectStuff".Translate(artStuff.label), Color.white))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach(var stuff in DefDatabase<ThingDef>.AllDefs)
                {
                    if(stuff.IsStuff && stuff.stuffProps.CanMake(artType))
                    {
                        options.Add(new FloatMenuOption(stuff.label, delegate
                        {
                            artStuff = stuff;
                        }));
                    }
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }
            buttonRect.y += 30;
            if (GUIUtils.DrawCustomButton(buttonRect, "RogerEdmonson_OrderWindow_SelectArtType".Translate(artType.label), Color.white))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var art in DefDatabase<ThingDef>.AllDefs)
                {
                    if (art.IsArt)
                    {
                        options.Add(new FloatMenuOption(art.label, delegate
                        {
                            artType = art;
                        }));
                    }
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }
            Text.Anchor = TextAnchor.UpperLeft;

            Rect intRect = new Rect(rect.x + 10, rect.y + 60, 250, 25);
            Widgets.Label(intRect, "RogerEdmonson_OrderWindow_Range".Translate());
            intRect.x += 255;
            intRect.width = rect.width - 275;
            Widgets.TextFieldNumeric(intRect, ref delay, ref delayBuff, 1);

            Rect fullLabel = new Rect(rect.x + 10, rect.y + 90, rect.width - 10, 300);
            Widgets.Label(fullLabel, "RogerEdmonson_OrderWindow_ArtFull".Translate(totalValue, chance.ToString("f2"), prepayment));
            TooltipHandler.TipRegion(fullLabel, "RogerEdmonson_OrderWindow_ArtFull2".Translate(artType.label, baseValue, artStuff.BaseMarketValue, delay, trader.ArriveTime, artType.costStuffCount, totalValue, prepayment
                , chance.ToString("f2"), baseChance));

            Text.Anchor = TextAnchor.MiddleCenter;
            if (GUIUtils.DrawCustomButton(new Rect(rect.x + 10, rect.height - 40, rect.width - 20, 30), "RogerEdmonson_OrderWindow_CreateOrder".Translate(), trader.Order == null ? Color.white : Color.gray))
            {
                if (trader.Order != null)
                {
                    Messages.Message("RogerEdmonson_OrderWindow_Only1Order".Translate(), MessageTypeDefOf.NeutralEvent, false);
                }
                else
                {
                    if (TakePrePayment(prepayment))
                    {
                        MakeArtOrder(chance, totalValue, delay, artType, artStuff);
                    }
                }
            }
            if (trader.Order != null)
            {
                if (GUIUtils.DrawCustomButton(new Rect(rect.x + 10, rect.height, rect.width - 20, 30), "DarkNetButtons_CancelOrder".Translate(), Color.white))
                {
                    trader.DeclineOrder();
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
