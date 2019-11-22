using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public static class GUIUtils
    {
        public static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        public static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        public static Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

        [TweakValue("Input", 0f, 100f)]
        private static int IntEntryButtonWidth;

        public static bool DrawCustomButton(Rect rect, string label, Color textColor)
        {
            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(rect);
            GUI.color = CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(rect);
            return Widgets.ButtonInvisible(rect);
        }

        public static bool DrawCustomButton(Rect rect, string label, Color textColor, string toolTip)
        {
            if(Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, toolTip);
            }

            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(rect);
            GUI.color = CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(rect);
            return Widgets.ButtonInvisible(rect);
        }

        public static void DrawLineHorizontal(float x, float y, float length, Color color)
        {
            GUI.color = color;
            Widgets.DrawLineHorizontal(x, y, length);
            GUI.color = Color.white;
        }

        public static void DrawLineVertical(float x, float y, float length, Color color)
        {
            GUI.color = color;
            Widgets.DrawLineVertical(x, y, length);
            GUI.color = Color.white;
        }

        public static void DrawQuestCard(Quest quest, List<Quest> quests, Rect rect)
        {
            bgCardColor.a = 150;
            Widgets.DrawBoxSolid(rect, bgCardColor);

            GUI.color = GUIUtils.CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x + 8, rect.y + 8, rect.width - 88, 25), quest.CardLabel);

            Text.Anchor = TextAnchor.UpperLeft;

            GUIUtils.DrawLineHorizontal(rect.x + 8, rect.y + 34, rect.width - 88, Color.gray);
            float y = rect.y + 36;
            Widgets.Label(new Rect(rect.x + 8, y, rect.width - 88, 150), quest.Description);

            Text.Anchor = TextAnchor.MiddleCenter;
            if (GUIUtils.DrawCustomButton(new Rect(rect.x + (rect.width - 200) / 2, rect.y + 175, 200, 25), "DarkNetButtons_TakeQuest".Translate(), Color.white))
            {
                quest.TakeQuestByQuester(null);

                quests.Remove(quest);
            }
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, quest.GetRewardsString());
            }
        }
    }
}
