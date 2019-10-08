using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public static class GUIUtils
    {
        public static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        public static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        public static Color bgCardColor = new ColorInt(25, 25, 25).ToColor;

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

        public static void DrawSelectArrows(SellableItemWithModif item, Rect rect)
        {
            if (GUIUtils.DrawCustomButton(rect, "-1", Color.white))
            {
                item.AddToTransfer(-1);
            }
            rect.x += 27;
            if (GUIUtils.DrawCustomButton(rect, "-5", Color.white))
            {
                item.AddToTransfer(-5);
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
                item.AddToTransfer(5);
            }
        }

        public static void DrawItemCard(SellableItemWithModif item, List<SellableItemWithModif> itemsList, Rect rect)
        {
            bgCardColor.a = 150;
            Widgets.DrawBoxSolid(rect, bgCardColor);

            GUI.color = GUIUtils.CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            Widgets.ThingIcon(new Rect(rect.x + 8, rect.y + 18, 64, 64), item.Item);
            if (Widgets.ButtonImage(new Rect(rect.x + 26, rect.y + 86, 24, 24), DarkNETWindow.Info, GUI.color))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(item.Item));
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x + 80, rect.y + 8, rect.width - 88, 25), "TraderWorker_Eisenberg_ItemLabel".Translate(item.Item.LabelNoCount, item.Item.stackCount, item.MarketValue));

            Text.Anchor = TextAnchor.UpperLeft;

            GUIUtils.DrawLineHorizontal(rect.x + 80, rect.y + 34, rect.width - 88, Color.gray);
            float y = rect.y + 36;
            Widgets.Label(new Rect(rect.x + 80, y, rect.width - 88, 120), $"TraderWorker_Eisenberg_Description".Translate(item.Item.DescriptionDetailed));

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect arrowRect = new Rect(rect.x + 10, rect.y + 165, 25, 25);
            GUIUtils.DrawSelectArrows(item, arrowRect);
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

                if (DarkNetPriceUtils.BuyAndDropItem(item, item.CountToTransfer, Find.AnyPlayerHomeMap))
                {
                    if (item.Item == null)
                        itemsList.Remove(item);

                    if (item.Item != null)
                    {
                        if (item.CountToTransfer > item.Item.stackCount)
                            item.AddToTransfer(item.Item.stackCount);
                    }

                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, "TraderWorker_RogerEdmonson_FullDesc".Translate(item.Item.LabelNoCount, item.Item.DescriptionFlavor, item.MarketValue));
            }
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
