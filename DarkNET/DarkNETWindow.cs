using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    [StaticConstructorOnStartup]
    public class DarkNETWindow : Window
    {
        private Vector2 commSlider = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(1100, 750);

        private static List<TabRecord> tabsList = new List<TabRecord>();
        private static List<TabRecord> tradeTabList = new List<TabRecord>();

        private static Color offlineImageColor => new Color(0, 0, 0, 0.5f);

        private Pawn speaker;

        private DarkNet darkNet;

        private DarkNetTrader currentTrader;

        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
        private static readonly Texture2D StorytellerHighlightTex = ContentFinder<Texture2D>.Get("UI/HeroArt/Storytellers/Highlight");
        private static readonly Texture2D FirstPageTexture = ContentFinder<Texture2D>.Get("Traders/main");

        public DarkNETWindow(Pawn speaker)
        {
            this.speaker = speaker;

            forcePause = true;
            doCloseX = true;

            darkNet = Current.Game.GetComponent<DarkNet>();

            foreach (var trader in darkNet.Traders)
            {
                trader.WindowOpen();
            }
        }


        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect traderLabelOnlineRect = new Rect(0, 0, 100, 30);
            Widgets.Label(traderLabelOnlineRect, "DarkNET_TradersOnline".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect tradersList = new Rect(0, 30, 100, 730);
            Rect traderRect = new Rect(10, 10, 80, 80);
            Rect scrollVertRectFact = new Rect(0, 0, tradersList.x, darkNet.Traders.Count * 90);

            Widgets.BeginScrollView(tradersList, ref commSlider, scrollVertRectFact, true);
            foreach (var trader in darkNet.Traders)
            {
                DrawTraderIcon(traderRect, trader);
                traderRect.y += 90;
            }
            Widgets.EndScrollView();

            Rect mainTraderRect = new Rect(112, 40, 950, 674);
            if (currentTrader != null)
            {
                DrawTraderInfo(currentTrader);

                if (currentTrader.OnlineRightNow)
                    currentTrader.DrawTraderShop(mainTraderRect, speaker);
                else
                    DrawOfflinePage(mainTraderRect);
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;

                //Widgets.Label(mainTraderRect, "DarkNETWindow_ToStartInfo".Translate());
                GUI.DrawTexture(mainTraderRect, FirstPageTexture);

                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }

            GUIUtils.DrawLineVertical(112, 0, inRect.height, Color.gray);
        }

        private void DrawOfflinePage(Rect rect)
        {
            GUI.DrawTexture(rect, currentTrader.def.OfflineBackground);
            //Text.Anchor = TextAnchor.MiddleCenter;
            //Text.Font = GameFont.Medium;

            //Widgets.Label(rect, "OFFLINE");

            //Text.Anchor = TextAnchor.UpperLeft;
            //Text.Font = GameFont.Small;
        }

        private void DrawTraderIcon(Rect rect, DarkNetTrader trader)
        {
            bool status = trader.Online || trader.OnlineEveryTime;
            if (Widgets.ButtonImage(rect, status ? trader.IconMenu : trader.IconOfflineMenu))
            {
                currentTrader = trader;
            }
            Widgets.DrawHighlightIfMouseover(rect);

            if (!status)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(rect, StorytellerHighlightTex);
                GUI.color = Color.white;
            }

            if (currentTrader == trader)
            {
                GUI.DrawTexture(rect, StorytellerHighlightTex);
            }
        }

        private void DrawTraderInfo(DarkNetTrader trader)
        {
            Text.Font = GameFont.Medium;
            Vector2 textSize = Text.CalcSize(currentTrader.Name);
            Rect labelRect = new Rect(130, 0, textSize.x, 35);
            Widgets.Label(labelRect, currentTrader.Name);
            Rect rect = new Rect(labelRect.x + textSize.x + 5, 0, 24, 24);
            if (Widgets.ButtonImage(rect, Info, GUI.color))
            {
                Find.WindowStack.Add(new Dialog_DarkNetTraderCard(trader));
            }
            TooltipHandler.TipRegion(rect, "DefTraderInfoTip".Translate());

            GUIUtils.DrawLineHorizontal(112, 32, 360, Color.gray);

            Text.Font = GameFont.Small;
        }
    }
}
