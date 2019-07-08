using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    [StaticConstructorOnStartup]
    public class GeoscapeWindow : Window
    {
        private Vector2 commSlider = Vector2.zero;
        private Vector2 commSliderQuests = Vector2.zero;
        private Vector2 commInfoSlider = Vector2.zero;
        private Vector2 commInfoQuestSlider = Vector2.zero;
        private Vector2 commButtonsCommSlider = Vector2.zero;
        private Vector2 commButtonsQuestsSlider = Vector2.zero;
        private Vector2 questRewardSlider = Vector2.zero;

        private enum Tab
        {
            Quests,
            Events,
            Interaction
        }

        public override Vector2 InitialSize => new Vector2(1000, 700);

        private Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private readonly List<CommunicationDialog> communicationsDialogs;
        private readonly List<Quest> quests;

        private Communications communications;

        private Pawn speaker;
        private Pawn defendant;
        private int commDialogSliderLength = 0;
        private int commQuestsSliderLength = 0;

        private CommunicationDialog currentDialog;
        private Quest currentQuest;

        private static readonly Color DisabledSkillColor = new Color(1f, 1f, 1f, 0.5f);
        private static Texture2D SkillBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        private static readonly Color JumpToLocationColor = new ColorInt(101, 172, 247).ToColor;

        public GeoscapeWindow(Communications communications, Pawn speaker)
        {
            communicationsDialogs = communications.CommunicationDialogs;
            quests = communications.Quests;


            this.communications = communications;
            this.speaker = speaker;

            commDialogSliderLength = communicationsDialogs.Count * 78;
            commQuestsSliderLength = quests.Count * 108;

            forcePause = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect2 = inRect;
            rect2.yMin += 35;
            tabsList.Clear();
            tabsList.Add(new TabRecord("EventsCommTab".Translate(), delegate
            {
                tab = Tab.Events;
                currentQuest = null;
            }, tab == Tab.Events));
            tabsList.Add(new TabRecord("QuestsCommTab".Translate(), delegate
            {
                tab = Tab.Quests;
                currentDialog = null;
            }, tab == Tab.Quests));
            tabsList.Add(new TabRecord("InteractionCommTab".Translate(), delegate
            {
                tab = Tab.Interaction;
            }, tab == Tab.Interaction));

            Widgets.DrawMenuSection(rect2);
            TabDrawer.DrawTabs(rect2, tabsList, maxTabWidth: 500);
            tabsList.Clear();

            switch (tab)
            {
                case Tab.Events:
                    EventsPage(rect2);
                    break;
                case Tab.Quests:
                    QuestsPage(rect2);
                    break;
                case Tab.Interaction:
                    InteractionPage(rect2);
                    break;
            }
        }

        private void EventsPage(Rect inRect)
        {
            Rect rect1 = inRect;
            Rect rect2 = inRect;
            rect1.xMax = 328;
            rect1.height = 500;

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(rect1);
            GUI.color = Color.white;

            Rect scrollVertRectFact = new Rect(0, 0, rect1.x, commDialogSliderLength);
            int y = 10;
            Widgets.BeginScrollView(rect1, ref commSlider, scrollVertRectFact, false);
            foreach(var comDialog in communicationsDialogs)
            {
                DrawCommCard(rect1, ref y, comDialog);
            }
            Widgets.EndScrollView();

            DrawPawnCard();

            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(340, rect2.yMin + 20, 610, rect2.yMax);
            DrawDialogCard(rect3);
        }

        private void DrawDialogCard(Rect inRect)
        {
            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(new Rect(327, 534, 640, 150));
            GUI.color = Color.white;

            if (currentDialog != null)
            {
                if (currentDialog.Options != null)
                {
                    int sliderLength = currentDialog.Options.Count * 40;
                    Rect buttonRect = new Rect(0, 0, 622, sliderLength);
                    Rect scrollVertRectFact = new Rect(0, 0, inRect.x, sliderLength);
                    Widgets.BeginScrollView(new Rect(332, 540, 622, 115), ref commButtonsCommSlider, scrollVertRectFact, false);
                    DoButtonsComms(buttonRect);
                    Widgets.EndScrollView();
                }

                Widgets.LabelScrollable(new Rect(340, inRect.y, 610, 465), currentDialog.Description, ref commInfoSlider, false, false);
            }
        }

        private void DoButtonsComms(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            foreach(var option in currentDialog.Options)
            {
                if(listing.ButtonText(option.Label))
                {
                    foreach(var action in option.Actions)
                    {
                        action.DoAction(currentDialog, speaker, defendant);
                    }
                }
            }
            listing.End();
        }

        private void DrawPawnCard()
        {
            GUI.DrawTexture(new Rect(0, 530, 100, 140), PortraitsCache.Get(speaker, new Vector2(100, 140)));
            Widgets.Label(new Rect(100, 550, 210, 30), speaker.Name.ToStringFull);
            Widgets.DrawLineHorizontal(100, 570, 210);

            Text.Font = GameFont.Tiny;
            DrawSkill(speaker.skills.GetSkill(SkillDefOf.Social), new Rect(100, 580, 210, 24));
            DrawSkill(speaker.skills.GetSkill(SkillDefOf.Intellectual), new Rect(100, 610, 210, 24));
        }

        private void DrawSkill(SkillRecord skill, Rect holdingRect)
        {
            if (Mouse.IsOver(holdingRect))
            {
                GUI.DrawTexture(holdingRect, TexUI.HighlightTex);
            }
            GUI.BeginGroup(holdingRect);
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rect = new Rect(6f, 0f, 90f, holdingRect.height);
            Widgets.Label(rect, skill.def.skillLabel.CapitalizeFirst());
            Rect position = new Rect(rect.xMax, 0f, 24f, 24f);
            if (!skill.TotallyDisabled)
            {
                Rect rect2 = new Rect(position.xMax, 0f, holdingRect.width - position.xMax, holdingRect.height);
                float fillPercent = Mathf.Max(0.01f, (float)skill.Level / 20f);
                Widgets.FillableBar(rect2, fillPercent, SkillBarFillTex, null, doBorder: false);
            }
            Rect rect3 = new Rect(position.xMax + 4f, 0f, 999f, holdingRect.height);
            rect3.yMin += 3f;
            string label;
            if (skill.TotallyDisabled)
            {
                GUI.color = DisabledSkillColor;
                label = "-";
            }
            else
            {
                label = skill.Level.ToStringCached();
            }
            GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
            Widgets.Label(rect3, label);
            GenUI.ResetLabelAlign();
            GUI.color = Color.white;
            GUI.EndGroup();
            string text = GetSkillDescription(skill);
            TooltipHandler.TipRegion(holdingRect, new TipSignal(text, skill.def.GetHashCode() * 397945));
        }

        private string GetSkillDescription(SkillRecord sk)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (sk.TotallyDisabled)
            {
                stringBuilder.Append("DisabledLower".Translate().CapitalizeFirst());
            }
            else
            {
                stringBuilder.AppendLine("Level".Translate() + " " + sk.Level + ": " + sk.LevelDescriptor);
                if (Current.ProgramState == ProgramState.Playing)
                {
                    string text = (sk.Level != 20) ? "ProgressToNextLevel".Translate() : "Experience".Translate();
                    stringBuilder.AppendLine(text + ": " + sk.xpSinceLastLevel.ToString("F0") + " / " + sk.XpRequiredForLevelUp);
                }
                stringBuilder.Append("Passion".Translate() + ": ");
                switch (sk.passion)
                {
                    case Passion.None:
                        stringBuilder.Append("PassionNone".Translate(0.35f.ToStringPercent("F0")));
                        break;
                    case Passion.Minor:
                        stringBuilder.Append("PassionMinor".Translate(1f.ToStringPercent("F0")));
                        break;
                    case Passion.Major:
                        stringBuilder.Append("PassionMajor".Translate(1.5f.ToStringPercent("F0")));
                        break;
                }
                if (sk.LearningSaturatedToday)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("LearnedMaxToday".Translate(sk.xpSinceMidnight.ToString("F0"), 4000, 0.2f.ToStringPercent("F0")));
                }
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append(sk.def.description);
            return stringBuilder.ToString();
        }


        private void DrawCommCard(Rect rect, ref int y, CommunicationDialog dialog)
        {
            Rect r = new Rect(10, y, rect.width - 20, 60);
            Widgets.Label(r, dialog.CardLabel);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(10, y + 22, rect.width - 20, 20);
            Widgets.Label(rect2, dialog.RelatedIncident != null ? "RelatedEventCom".Translate(dialog.RelatedIncident.LabelCap) : "NoEventComm".Translate());
            rect2.y += 20;
            Widgets.Label(rect2, dialog.Faction != null ? "FactionComm".Translate(dialog.Faction.Name) : "NoFactionComm".Translate());
            Text.Font = GameFont.Small;
            Widgets.DrawHighlightIfMouseover(r);

            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;
            if(Widgets.ButtonInvisible(r))
            {
                currentDialog = dialog;
            }
            y += 77;
        }

        private void QuestsPage(Rect inRect)
        {
            Rect rect1 = inRect;
            Rect rect2 = inRect;
            rect1.xMax = 319;
            rect1.height = 500;

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(rect1);
            GUI.color = Color.white;

            Rect scrollVertRectFact = new Rect(0, 0, rect1.x, commQuestsSliderLength);
            int y = 10;
            Widgets.BeginScrollView(rect1, ref commSliderQuests, scrollVertRectFact, false);
            foreach (var quest in quests)
            {
                DrawQuestCard(rect1, ref y, quest);
            }
            Widgets.EndScrollView();

            DrawPawnCard();

            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(328, rect2.yMin + 20, 610, rect2.yMax);
            DrawQuestCard(rect3);
        }

        private void DrawQuestCard(Rect rect, ref int y, Quest quest)
        {
            Rect r = new Rect(10, y, rect.width - 20, 90);
            Widgets.Label(r, quest.CardLabel);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(10, y + 22, rect.width - 20, 20);
            Widgets.Label(rect2, quest.Faction != null ? "FactionComm".Translate(quest.Faction.Name) : "NoFactionComm".Translate());
            rect2.y += 20;
            Widgets.Label(rect2, quest.UnlimitedTime ? "UnlimitedTime".Translate() : "QuestTimer".Translate(GenDate.TicksToDays(quest.TicksToPass).ToString("f2")));
            rect2.y += 25;
            if (quest.Target != null)
            {
                GUI.color = JumpToLocationColor;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.DrawHighlightIfMouseover(rect2);
                Widgets.Label(rect2, "JumpToLocation".Translate(quest.Target.TryGetPrimaryTarget().ToString()));
                if(Widgets.ButtonInvisible(rect2))
                {
                    GlobalTargetInfo target = quest.Target.TryGetPrimaryTarget();
                    CameraJumper.TryJumpAndSelect(target);
                    Close();
                }
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            Text.Font = GameFont.Small;
            Widgets.DrawHighlightIfMouseover(r);

            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;
            if (Widgets.ButtonInvisible(r))
            {
                currentQuest = quest;
            }
            y += 107;
        }

        private void DrawQuestCard(Rect inRect)
        {
            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(new Rect(318, 534, 646, 150));
            GUI.color = Color.white;

            if (currentQuest != null)
            {
                if (currentQuest.Options != null)
                {
                    int sliderLength = currentQuest.Options.Count * 40;
                    Rect buttonRect = new Rect(0, 0, 622, sliderLength);
                    Rect scrollVertRectFact = new Rect(0, 0, inRect.x, sliderLength);
                    Widgets.BeginScrollView(new Rect(332, 540, 622, 115), ref commButtonsQuestsSlider, scrollVertRectFact, false);
                    DoButtonsQuest(buttonRect);
                    Widgets.EndScrollView();
                }

                Widgets.LabelScrollable(new Rect(330, inRect.y, 620, 230), currentQuest.Description, ref commInfoQuestSlider, false, false);

                GUI.color = MenuSectionBGBorderColor;
                Widgets.DrawLineHorizontal(318, 298, 646);
                Widgets.DrawLineVertical(641, 298, 236);
                Widgets.DrawLineHorizontal(330, 330, 290);
                Widgets.DrawLineHorizontal(660, 330, 290);
                GUI.color = Color.white;

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(330, 310, 300, 20), "QuestRewards".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                int questSliderLength = currentQuest.Rewards.Count * 30;
                Rect rewardsRect = new Rect(0, 0, 323, questSliderLength);
                Rect scrollRewVertRectFact = new Rect(0, 0, inRect.x, questSliderLength);
                Widgets.BeginScrollView(new Rect(330, 340, 300, 180), ref questRewardSlider, scrollRewVertRectFact, false);
                DrawQuestRewards(rewardsRect, currentQuest);
                Widgets.EndScrollView();

                Rect rectAdd = new Rect(660, 340, 300, 180);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(660, 310, 300, 20), string.IsNullOrEmpty(currentQuest.AdditionalQuestContentString) ? "AdditionalQuestContent".Translate() : currentQuest.AdditionalQuestContentString);
                Text.Anchor = TextAnchor.UpperLeft;
                currentQuest.DrawAdditionalOptions(rectAdd);
            }
        }

        public void DrawQuestRewards(Rect rect, Quest quest)
        {
            Text.Font = GameFont.Small;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            foreach (var reward in quest.Rewards)
            {
                listing.Label(reward.Label, 25, reward.DescriptionFlavor);
            }
            listing.End();
        }

        private void DoButtonsQuest(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            foreach (var option in currentQuest.Options)
            {
                if (listing.ButtonText(option.Label))
                {
                    foreach (var action in option.Actions)
                    {
                        action.DoAction(currentQuest, speaker, defendant);
                    }
                }
            }
            listing.End();
        }

        private void InteractionPage(Rect inRect)
        {

        }
    }
}
