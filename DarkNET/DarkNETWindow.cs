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

        public DarkNETWindow(Pawn speaker)
        {
            this.speaker = speaker;

            forcePause = true;
            doCloseX = true;
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

            
        }
    }
}
