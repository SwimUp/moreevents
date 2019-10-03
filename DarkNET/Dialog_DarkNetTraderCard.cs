using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class Dialog_DarkNetTraderCard : Window
    {
        public override Vector2 InitialSize => new Vector2(900, 700);

        private DarkNetTrader trader;

        private Vector2 scroll = Vector2.zero;

        private float width;

        public Dialog_DarkNetTraderCard(DarkNetTrader trader)
        {
            doCloseX = true;
            this.trader = trader;

            if (trader.def.OverridePortraitWidth == 0)
                width = 400;
            else
                width = trader.def.OverridePortraitWidth;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect textureRect = new Rect(400, 0, width, inRect.height);
            GUI.DrawTexture(textureRect, trader.def.FullTexture);

            Rect title = new Rect(0, 0, 500, 30);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(title, trader.Name);
            GUIUtils.DrawLineHorizontal(100, 30, 200, Color.gray);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.LabelScrollable(new Rect(0, 35, 390, 630), trader.Description, ref scroll);
        }
    }
}
