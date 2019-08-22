using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class DarkNETWindow : Window
    {
        private Vector2 commSlider = Vector2.zero;

        private enum Tab
        {
            Trade
        }

        private enum TradeTabs
        {
            Pawns,
            Weapons,
            Medicine,
            Other
        }

        public override Vector2 InitialSize => new Vector2(1100, 750);

        private Tab tab;
        private TradeTabs tradeTab;

        private static List<TabRecord> tabsList = new List<TabRecord>();
        private static List<TabRecord> tradeTabList = new List<TabRecord>();

        private Pawn speaker;

        private static Dictionary<Tab,  List<DarkNetTrader>> tradersData;

        private static DarkNet darkNet;

        public DarkNETWindow(Pawn speaker)
        {
            this.speaker = speaker;

            forcePause = true;
            doCloseX = true;

            if(tradersData == null)
            {
                CreateTraderList();
            }

            if(darkNet == null)
            {
                darkNet = Current.Game.GetComponent<DarkNet>();
            }
        }

        private void CreateTraderList()
        {
            tradersData = new Dictionary<Tab, List<DarkNetTrader>>();

            foreach(Tab type in Enum.GetValues(typeof(Tab)))
            {
                tradersData.Add(type, darkNet.Traders.Where(x => x.def.TraderType == TraderType.Trader).ToList());
            }
        }


        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord("DarkNET_TradeTab".Translate(), delegate
            {
                tab = Tab.Trade;
            }, tab == Tab.Trade));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 500);
            tabsList.Clear();

            switch (tab)
            {
                case Tab.Trade:
                    DrawTradeTab(rect2);
                    break;
            }
        }

        private void DrawTradeTab(Rect rect)
        {
            Rect main = rect;
            main.y += 32;

            tradeTabList.Clear();
            tradeTabList.Add(new TabRecord("DarkNET_TradeTab_Peoples".Translate(), delegate
            {
                tradeTab = TradeTabs.Pawns;
            }, tradeTab == TradeTabs.Pawns));
            tradeTabList.Add(new TabRecord("DarkNET_TradeTab_Weapons".Translate(), delegate
            {
                tradeTab = TradeTabs.Weapons;
            }, tradeTab == TradeTabs.Weapons));
            tradeTabList.Add(new TabRecord("DarkNET_TradeTab_Medicine".Translate(), delegate
            {
                tradeTab = TradeTabs.Medicine;
            }, tradeTab == TradeTabs.Medicine));
            tradeTabList.Add(new TabRecord("DarkNET_TradeTab_Other".Translate(), delegate
            {
                tradeTab = TradeTabs.Other;
            }, tradeTab == TradeTabs.Other));

            Widgets.DrawMenuSection(main);
            TabDrawer.DrawTabs(main, tradeTabList, maxTabWidth: 500);
            tradeTabList.Clear();

            switch (tradeTab)
            {
                case TradeTabs.Pawns:

                    break;
            }

            List<DarkNetTrader> traders = tradersData[Tab.Trade].Where(x => (int)x.def.Category == (int)tradeTab).ToList();
            int sliderLength = traders.Count * 40;

            Rect buttonRect = new Rect(0, 0, 50, 50);

            Rect scrollVertRectFact = new Rect(0, 0, sliderLength, rect.y);
            Widgets.BeginScrollView(new Rect(0, 690, 1100, 60), ref commSlider, scrollVertRectFact, true);
            foreach (var trader in traders)
            {
                DrawTraderButton(buttonRect, trader);
                buttonRect.x += 60;
            }
            Widgets.EndScrollView();
        }
        private void DrawTraderButton(Rect rect, DarkNetTrader trader)
        {
            Widgets.DrawBoxSolid(rect, new ColorInt(100, 100, 100).ToColor);
        }
    }
}
