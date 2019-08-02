using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class Dialog_QuestDialog : Window
    {
        public QuestPawn QuestPawn;
        public Pawn Quester => QuestPawn.Pawn;
        public Pawn Speaker;

        private Vector2 scroll = Vector2.zero;

        private Quest currentQuest = null;

        public override Vector2 InitialSize => new Vector2(900, 710);
        private Vector2 commButtonsCommSlider = Vector2.zero;
        private Vector2 commInfoSlider = Vector2.zero;
        private Vector2 questRewardSlider = Vector2.zero;
        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        private static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;
        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;

        public Dialog_QuestDialog(QuestPawn quester, Pawn speaker)
        {
            QuestPawn = quester;
            Speaker = speaker;
            forcePause = true;
            doCloseX = true;
        }
        public override void DoWindowContents(Rect inRect)
        {
            if (QuestPawn.Quests.Count == 0)
                Close();

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(0, 0, inRect.width, 50);
            Widgets.Label(titleRect, "QuestPawn_Title".Translate());
            Text.Font = GameFont.Small;
            Rect mainRect = inRect;
            mainRect.y = 35;
            mainRect.height = 30;
            if(Quester.Faction != Faction.OfPlayer)
            {
                Widgets.Label(mainRect, $"QuestPawn_Line1".Translate(Quester.Name.ToStringFull, Quester.Faction.Name, Quester.Faction.PlayerRelationKind.GetLabel(), Quester.Faction.PlayerGoodwill.ToStringWithSign()));
            }
            else
            {
                Widgets.Label(mainRect, $"QuestPawn_Line1_Player".Translate(Quester.Name.ToStringFull));
            }
            GUI.color = CommCardBGColor;
            Widgets.DrawLineHorizontal(0, 55, inRect.width);
            GUI.color = Color.white;

            Rect scrollRect = inRect;
            scrollRect.x = 10;
            scrollRect.width = 880;
            scrollRect.y = 70;
            scrollRect.height = 100;
            Rect scrollVertRectFact = new Rect(0, 0, inRect.width, QuestPawn.Quests.Count * 25);
            Widgets.BeginScrollView(scrollRect, ref scroll, scrollVertRectFact, true);
            int elemInRow = Mathf.Max(1, (QuestPawn.Quests.Count + 1) / 2);
            int row = 0;
            Rect buttonRect = new Rect(0, 0, 410, 20);
            Text.Anchor = TextAnchor.MiddleCenter;
            for(int i = 0; i < QuestPawn.Quests.Count; i++)
            {
                Quest quest = QuestPawn.Quests[i];

                if(row == elemInRow)
                {
                    buttonRect.x = 430;
                    buttonRect.y = 0;
                }

                if(DrawCustomButton(buttonRect, quest.CardLabel, Color.white))
                {
                    currentQuest = quest;
                }
                buttonRect.y += 25;
                row++;
            }
            Widgets.EndScrollView();
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = CommCardBGColor;
            Widgets.DrawLineHorizontal(0, 170, inRect.width);
            GUI.color = Color.white;

            if(currentQuest != null)
            {
                if(!QuestPawn.Quests.Contains(currentQuest))
                {
                    currentQuest = null;
                    return;
                }

                if (currentQuest.Options != null)
                {
                    int sliderLength = currentQuest.Options.Count * 40;
                    Rect buttonRect2 = new Rect(0, 0, inRect.width, sliderLength);
                    Rect scrollVertRectFact2 = new Rect(0, 0, inRect.width, sliderLength);
                    Widgets.BeginScrollView(new Rect(0, 495, inRect.width, 115), ref commButtonsCommSlider, scrollVertRectFact2, false);
                    DoButtonsQuest(buttonRect2);
                    Widgets.EndScrollView();
                }

                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;
                if (DrawCustomButton(new Rect(0, 625, inRect.width, 40), "TakeQuestFromPawn".Translate(), Color.yellow))
                {
                    currentQuest.TakeQuestByQuester(QuestPawn);
                    currentQuest = null;
                    Text.Anchor = TextAnchor.UpperLeft;
                    return;
                }
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;

                Widgets.LabelScrollable(new Rect(0, 185, inRect.width, 130), currentQuest.Description, ref commInfoSlider, false, false);

                GUI.color = MenuSectionBGBorderColor;
                Widgets.DrawLineHorizontal(0, 320, inRect.width);
                float width = inRect.width / 2;
                Widgets.DrawLineVertical(width, 320, 170);
                Widgets.DrawLineHorizontal(60, 349, 290);
                Widgets.DrawLineHorizontal(510, 349, 290);
                Widgets.DrawLineHorizontal(0, 490, inRect.width);
                GUI.color = Color.white;

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(40, 330, 300, 20), "QuestRewards".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                int questSliderLength = currentQuest.Rewards.Count * 30;
                Rect rewardsRect = new Rect(0, 0, 390, questSliderLength);
                Rect scrollRewVertRectFact = new Rect(0, 0, inRect.x, questSliderLength);
                Widgets.BeginScrollView(new Rect(0, 355, 410, 125), ref questRewardSlider, scrollRewVertRectFact, false);
                DrawQuestRewards(rewardsRect, currentQuest);
                Widgets.EndScrollView();

                Rect rectAdd = new Rect(460, 355, 410, 125);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(460, 330, 410, 20), string.IsNullOrEmpty(currentQuest.AdditionalQuestContentString) ? "AdditionalQuestContent".Translate() : currentQuest.AdditionalQuestContentString);
                Text.Anchor = TextAnchor.UpperLeft;
                currentQuest.DrawAdditionalOptions(rectAdd);
            }
        }

        private void DoButtonsQuest(Rect rect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            Rect startRect = rect;
            startRect.height = 35;
            for(int i = 0; i < currentQuest.Options.Count; i++)
            {
                QuestOption option = currentQuest.Options[i];

                if (option.Enable)
                {
                    if (DrawCustomButton(startRect, option.Label, option.TextColor))
                    {
                        option.DoAction(currentQuest, Speaker, Quester);
                    }
                    startRect.y += 40;
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void DrawQuestRewards(Rect rect, Quest quest)
        {
            Text.Font = GameFont.Small;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            for(int i = 0; i < quest.Rewards.Count; i++)
            {
                Thing reward = quest.Rewards[i];

                listing.Label(reward.Label, 25, reward.DescriptionFlavor);
            }
            listing.End();
        }

        private bool DrawCustomButton(Rect rect, string label, Color textColor)
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
    }
}
