﻿using RimWorld;
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
        private Vector2 factionListSlider = Vector2.zero;

        public enum Tab
        {
            Quests,
            Events,
            Interaction
        }

        private enum InterTab
        {
            Interaction,
            Messages,
            Alliance
        }

        public override Vector2 InitialSize => new Vector2(1000, 700);

        public Tab tab;
        private InterTab interTab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private readonly List<CommunicationDialog> communicationsDialogs;
        private readonly List<Quest> quests;
        private readonly List<FactionInteraction> factions;
        private readonly List<EmailMessage> emailMessages;
        private readonly List<Alliance> alliances;

        private Communications communications;

        private Pawn speaker;
        private Pawn defendant;
        private int commDialogSliderLength = 0;
        private int commQuestsSliderLength = 0;
        private int commFactionSliderLength = 0;
        private int commAllianceSliderLength = 0;
        private int commEmailsSliderLength = 0;

        public CommunicationDialog currentDialog;
        public Quest currentQuest;
        public FactionInteraction currentFaction;
        public EmailMessage currentMessage;
        public FactionManager globalFactionManager;

        public static readonly Color DisabledSkillColor = new Color(1f, 1f, 1f, 0.5f);
        public static Texture2D SkillBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
        public static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
        public static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        public static readonly Color CommCardBGColorNotRead = new ColorInt(166, 80, 80).ToColor;
        public static readonly Color JumpToLocationColor = new ColorInt(101, 172, 247).ToColor;
        public static readonly Color InterBottomColor = new ColorInt(163, 130, 95).ToColor;
        public static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;

        private Dictionary<FactionInteraction, War> hasWarsWithPlayer = new Dictionary<FactionInteraction, War>();

        private float rewardCost = 0;

        public GeoscapeWindow(Communications communications, Pawn speaker)
        {
            globalFactionManager = QuestsManager.Communications.FactionManager;

            communicationsDialogs = communications.CommunicationDialogs;
            quests = communications.Quests;
            factions = communications.FactionManager.Factions.Where(x => x != null && x.Faction != null && !x.Faction.def.isPlayer && !x.Faction.def.hidden).ToList();
            emailMessages = communications.PlayerBox.Messages;
            alliances = globalFactionManager.Alliances;
            globalFactionManager.Alliances.SortByDescending(x => x.PlayerOwner);

            foreach (var fac in factions)
            {
                hasWarsWithPlayer.Add(fac, fac.InWars.FirstOrDefault(x => x.DeclaredWarFaction.Faction == speaker.Faction && x.DefendingFaction == fac || x.DefendingFaction.Faction == speaker.Faction && x.DeclaredWarFaction == fac));
            }

            this.communications = communications;
            this.speaker = speaker;

            commDialogSliderLength = communicationsDialogs.Count * 78;
            commQuestsSliderLength = quests.Count * 108;
            commFactionSliderLength = factions.Count * 85;
            commEmailsSliderLength = emailMessages.Count * 130;
            commAllianceSliderLength = alliances.Count * 95;

            forcePause = true;
            doCloseX = true;
        }

        public void UpdateWarsStatus()
        {
            foreach (var fac in factions)
            {
                hasWarsWithPlayer[fac] = fac.InWars.FirstOrDefault(x => x.DeclaredWarFaction.Faction == speaker.Faction && x.DefendingFaction == fac || x.DefendingFaction.Faction == speaker.Faction && x.DeclaredWarFaction == fac);
            }
        }

        public void ForceSelectQuest(Quest quest)
        {
            tab = Tab.Quests;
            currentQuest = quest;

            rewardCost = 0;

            if (currentQuest.Rewards != null)
            {
                foreach (var k in currentQuest.Rewards)
                    rewardCost += k.def.BaseMarketValue * k.stackCount;
            }
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
            for(int i = 0; i < communicationsDialogs.Count; i++)
            {
                CommunicationDialog comDialog = communicationsDialogs[i];

                if(comDialog.ShowInConsole)
                    DrawCommCard(rect1, ref y, comDialog);
            }
            Widgets.EndScrollView();

            DrawPawnCard(speaker);

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
                    Rect buttonRect = new Rect(0, 0, 620, sliderLength);
                    Rect scrollVertRectFact = new Rect(0, 0, inRect.x, sliderLength);
                    Widgets.BeginScrollView(new Rect(334, 540, 620, 115), ref commButtonsCommSlider, scrollVertRectFact, false);
                    DoButtonsComms(buttonRect);
                    Widgets.EndScrollView();
                }

                Widgets.LabelScrollable(new Rect(340, inRect.y, 610, 465), currentDialog.Description, ref commInfoSlider, false, false);
            }
        }

        private void DoButtonsComms(Rect rect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            Rect startRect = rect;
            startRect.height = 35;
            for(int i = 0; i < currentDialog.Options.Count; i++)
            {
                CommOption option = currentDialog.Options[i];

                if(DrawCustomButton(startRect, option.Label, option.TextColor))
                {
                    option.DoAction(currentDialog, speaker, defendant);
                }
                startRect.y += 40;
            }

            Text.Anchor = TextAnchor.UpperLeft;
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

        public void DrawPawnCard(Pawn pawn)
        {
            GUI.DrawTexture(new Rect(0, 530, 100, 140), PortraitsCache.Get(pawn, new Vector2(100, 140)));
            Widgets.Label(new Rect(100, 550, 210, 30), pawn.Name.ToStringFull);
            Widgets.DrawLineHorizontal(100, 570, 210);

            Text.Font = GameFont.Tiny;
            DrawSkill(pawn.skills.GetSkill(SkillDefOf.Social), new Rect(100, 580, 210, 24));
            DrawSkill(pawn.skills.GetSkill(SkillDefOf.Intellectual), new Rect(100, 610, 210, 24));
        }

        public static void DrawSkill(SkillRecord skill, Rect holdingRect)
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

        public static string GetSkillDescription(SkillRecord sk)
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
            Rect titleRect = new Rect(15, y, rect.width - 20, 60);
            string title = dialog.CardLabel.Length > 36 ? dialog.CardLabel.Substring(0, 36) : dialog.CardLabel;
            Widgets.Label(titleRect, title);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 22, rect.width - 20, 20);
            Widgets.Label(rect2, dialog.RelatedIncident != null ? "RelatedEventCom".Translate(dialog.RelatedIncident.LabelCap) : "NoEventComm".Translate());
            rect2.y += 20;
            Widgets.Label(rect2, dialog.Faction != null ? "FactionComm".Translate(dialog.Faction.Name) : dialog.KnownFaction ? "NoFactionComm".Translate() : "NoFactionComm_D".Translate());
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
            for(int i = 0; i < quests.Count; i++)
            {
                Quest quest = quests[i];

                if(quest.ShowInConsole)
                    DrawQuestCard(rect1, ref y, quest);
            }
            Widgets.EndScrollView();

            DrawPawnCard(speaker);

            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(328, rect2.yMin + 20, 610, rect2.yMax);
            DrawQuestCard(rect3);
        }

        private void DrawQuestCard(Rect rect, ref int y, Quest quest)
        {
            Rect r = new Rect(10, y, rect.width - 20, 90);
            Rect titleRect = new Rect(15, y, rect.width - 20, 90);
            string questLabel = quest.CardLabel.Length > 36 ? quest.CardLabel.Substring(0, 36) : quest.CardLabel;
            Widgets.Label(titleRect, questLabel);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 22, rect.width - 20, 20);
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
            if (currentQuest == quest)
            {
                Widgets.DrawHighlight(r);
                GUI.color = quest.Faction.Color;
                Widgets.DrawBox(r);
                GUI.color = Color.white;
            }
            else
            {
                Widgets.DrawHighlightIfMouseover(r);
            }

            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;
            if (Widgets.ButtonInvisible(r))
            {
                rewardCost = 0;

                currentQuest = quest;

                if (currentQuest.Rewards != null)
                {
                    foreach (var k in currentQuest.Rewards)
                        rewardCost += k.def.BaseMarketValue * k.stackCount;
                }
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
                Widgets.Label(new Rect(330, 310, 300, 20), "QuestRewards".Translate(rewardCost));
                Text.Anchor = TextAnchor.UpperLeft;
                if (currentQuest.Rewards != null)
                {
                    int questSliderLength = currentQuest.Rewards.Count * 30;
                    Rect rewardsRect = new Rect(0, 0, 323, questSliderLength);
                    Rect scrollRewVertRectFact = new Rect(0, 0, inRect.x, questSliderLength);
                    Widgets.BeginScrollView(new Rect(330, 340, 300, 180), ref questRewardSlider, scrollRewVertRectFact, false);
                    DrawQuestRewards(rewardsRect, currentQuest);
                    Widgets.EndScrollView();
                }

                Rect rectAdd = new Rect(660, 340, 300, 180);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(660, 310, 300, 20), string.IsNullOrEmpty(currentQuest.AdditionalQuestContentString) ? "AdditionalQuestContent".Translate().ToString() : currentQuest.AdditionalQuestContentString);
                Text.Anchor = TextAnchor.UpperLeft;
                currentQuest.DrawAdditionalOptions(rectAdd);
            }
        }

        public void DrawQuestRewards(Rect rect, Quest quest)
        {
            Text.Font = GameFont.Small;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            for (int i = 0; i < quest.Rewards.Count; i++)
            {
                Thing reward = quest.Rewards[i];

                listing.Label(reward.Label, 25, reward.DescriptionFlavor);
            }
            listing.End();
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
                        option.DoAction(currentQuest, speaker, defendant);
                    }
                    startRect.y += 40;
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;

        }

        private void InteractionPage(Rect inRect)
        {
            Rect rect2 = inRect;
            rect2.y = 590;
            rect2.height = 60;
            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineHorizontal(0, rect2.y, rect2.width);
            GUI.color = Color.white;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect rect3 = rect2;
            rect3.y = 600;
            rect3.height = 50;
            rect3.width = 280;
            rect3.x = 20;
            DrawBottomInterButtons(ref rect3, "InteractionInterTab".Translate(), () => { interTab = InterTab.Interaction; });
            rect3.x = 330;
            rect3.width = 300;
            DrawBottomInterButtons(ref rect3, "MessagesInterTab".Translate(), () => { interTab = InterTab.Messages; currentMessage = null; });
            rect3.x = 660;
            rect3.width = 280;
            DrawBottomInterButtons(ref rect3, "AllianceInterTab".Translate(), () => interTab = InterTab.Alliance);

            switch (interTab)
            {
                case InterTab.Interaction:
                    DrawInteraction(inRect);
                    break;
                case InterTab.Messages:
                    DrawEmailBox(inRect);
                    break;
                case InterTab.Alliance:
                    DrawAllianceMainPage(inRect);
                    break;
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawEmailBox(Rect inRect)
        {
            if (currentMessage == null)
            {
                DrawEmailMessages(inRect);
            }
            else
            {
                DrawEmailMessage(inRect);
            }
        }

        private void DrawEmailMessage(Rect rect)
        {
            rect.height = 560;
            rect.y = 20;

            Text.Anchor = TextAnchor.UpperLeft;

            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(15, rect.y + 15, rect.width - 20, 50);
            Widgets.Label(titleRect, currentMessage.Subject);
            Text.Font = GameFont.Small;
            Rect headerRect = rect;
            headerRect.height = 22;
            headerRect.y = 60;
            headerRect.x = 15;
            Widgets.Label(headerRect, "EmailMessage_From".Translate(currentMessage.From));
            headerRect.y += 22;
            Widgets.Label(headerRect, "EmailMessage_To".Translate(currentMessage.To));
            headerRect.y += 20;
            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawLineHorizontal(0, headerRect.y + 3, rect.width);
            GUI.color = Color.white;

            Rect textRect = rect;
            textRect.x = 15;
            textRect.y = headerRect.y + 10;
            textRect.width -= 20;
            textRect.height = 465;
            Widgets.LabelScrollable(textRect, currentMessage.Message, ref commInfoSlider);
        }

        private void DrawEmailMessages(Rect inRect)
        {
            Rect rect = inRect;
            rect.height = 560;
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, commEmailsSliderLength);
            int y = 10;
            Widgets.BeginScrollView(rect, ref commSlider, scrollVertRectFact, false);
            for(int i = 0; i < emailMessages.Count; i++)
            {
                EmailMessage message = emailMessages[i];
                DrawEmailMessage(rect, ref y, message);
            }
            Widgets.EndScrollView();
        }

        private void DrawEmailMessage(Rect rect, ref int y, EmailMessage message)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Medium;

            Rect r = new Rect(10, y, rect.width - 20, 120);
            Rect titleRect = new Rect(15, y, rect.width - 20, 50);
            Widgets.Label(titleRect, message.Subject);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 26, rect.width - 20, 20);
            Widgets.Label(rect2, "EmailMessage_From".Translate(message.From));
            rect2.y += 15;
            Widgets.Label(rect2, "EmailMessage_To".Translate(message.To));
            rect2.y += 25;

            Text.Anchor = TextAnchor.MiddleCenter;
            if (message.Answers != null)
            {
                for (int i = 0; i < Mathf.Min(4, message.Answers.Count); i++)
                {
                    EmailMessageOption answer = message.Answers[i];

                    if (DrawCustomButton(new Rect(rect2.x, rect2.y, 200, rect2.height), answer.Label, message.Answered ? Color.gray : Color.white))
                    {
                        if (message.Answered)
                            return;

                        message.Answered = true;
                        answer.DoAction(message, QuestsManager.Communications.PlayerBox, message.Faction?.leader);
                    }
                    rect2.x += 220;
                }
            }

            if (DrawCustomButton(new Rect(15, rect2.y + 25, 200, rect2.height), "DeleteMessage".Translate(), Color.white))
            {
                communications.PlayerBox.DeleteMessage(message);
            }

            Text.Anchor = TextAnchor.UpperLeft;
            
            Text.Font = GameFont.Small;
            if(Mouse.IsOver(r))
            {
                Widgets.DrawBox(r);
            }

            string str = $"{message.Message.Substring(0, message.Message.Length > 150 ? 150 : message.Message.Length)}...";
            TooltipHandler.TipRegion(r, str);

            GUI.color = message.MessageRead ? CommCardBGColor : CommCardBGColorNotRead;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(r))
            {
                currentMessage = message;
                currentMessage.MessageRead = true;
            }
            y += 130;
        }

        private void DrawBottomInterButtons(ref Rect rect, string text, Action click)
        {
            Widgets.DrawHighlight(rect);
            Widgets.Label(rect, text);

            if(Mouse.IsOver(rect))
            {
                GUI.color = InterBottomColor;
                Widgets.DrawBox(rect, 2);
                GUI.color = Color.white;
            }

            if(Widgets.ButtonInvisible(rect))
            {
                click.Invoke();
            }
        }

        private void DrawInteraction(Rect inRect)
        {
            Rect rect1 = inRect;
            Rect rect2 = inRect;
            rect1.xMax = 319;
            rect1.height = 556;

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(rect1);
            GUI.color = Color.white;

            Rect scrollVertRectFact = new Rect(0, 0, rect1.x, commFactionSliderLength);
            int y = 10;
            Widgets.BeginScrollView(rect1, ref factionListSlider, scrollVertRectFact, false);
            for(int i = 0; i < factions.Count; i++)
            {
                FactionInteraction faction = factions[i];

                DrawFactionCard(rect1, ref y, faction);
            }
            Widgets.EndScrollView();

            Rect rect3 = new Rect(328, rect2.yMin + 20, 610, rect2.yMax);
            DrawFactionCard(rect3);
        }

        private void DrawFactionCard(Rect rect)
        {
            if (currentFaction != null)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.UpperCenter;
                GUI.color = currentFaction.Faction.PlayerRelationKind.GetColor();
                Widgets.Label(new Rect(rect.x, rect.y - 10, rect.width + 10, 40), currentFaction.Faction.Name);
                GUI.color = Color.white;
                Widgets.DrawLineHorizontal(rect.x + (rect.x / 2), rect.y + 20, 280);
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
                rect.y += 30;

                Widgets.LabelScrollable(new Rect(rect.x, rect.y, rect.width, 180), currentFaction.Faction.def.description, ref commInfoSlider, false, false);

                Rect rect2 = new Rect(rect.x, rect.y + 200, rect.width / 2, 20);
                Widgets.Label(rect2, "FactionInteractionDef".Translate(currentFaction.Faction.def.LabelCap));
                rect2.y += 22;
                Widgets.Label(rect2, $"{currentFaction.Faction.def.leaderTitle.CapitalizeFirst()}: {currentFaction.Faction.leader.Name}");
                rect2.y += 30;
                StringBuilder builder = new StringBuilder();
                foreach (Faction item in Find.FactionManager.AllFactionsInViewOrder)
                {
                    if (item != currentFaction.Faction && (!item.IsPlayer && !item.def.hidden) && currentFaction.Faction.HostileTo(item))
                    {
                        builder.Append("HostileTo".Translate(item.Name));
                        builder.AppendLine();
                    }
                }
                Widgets.LabelScrollable(new Rect(rect2.x, rect2.y, rect2.width, 285), builder.ToString(), ref commSlider);

                Rect rect3 = new Rect((rect.x + rect.width / 2) + 10, rect.y + 200, rect.width / 2, 285);
                GUI.color = CommCardBGColor;
                Widgets.DrawLineVertical(rect.x + rect.width / 2, rect3.y, 285);
                GUI.color = Color.white;
                int sliderLength = currentFaction.Options.Count * 30;
                Rect buttonRect = new Rect(0, 0, rect3.width, sliderLength);
                Rect scrollVertRectFact = new Rect(0, 0, rect3.x, sliderLength);
                Widgets.BeginScrollView(rect3, ref commButtonsQuestsSlider, scrollVertRectFact, false);
                DrawInteractionButtons(buttonRect);
                Widgets.EndScrollView();
            }
        }

        private void DrawInteractionButtons(Rect rect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            Rect startRect = rect;
            startRect.height = Mathf.Max(25, Text.CalcHeight("CallOnRadio".Translate(currentFaction.Faction.Name), startRect.width));
            if(DrawCustomButton(startRect, "CallOnRadio".Translate(currentFaction.Faction.Name), Color.white))
            {
                Dialog_Negotiation dialog_Negotiation = new Dialog_Negotiation(speaker, currentFaction.Faction, FactionDialogMaker.FactionDialogFor(speaker, currentFaction.Faction), radioMode: true);
                dialog_Negotiation.soundAmbient = SoundDefOf.RadioComms_Ambience;
                Find.WindowStack.Add(dialog_Negotiation);
            }

            startRect.y += startRect.height + 5;
            startRect.height = 25;
            foreach (var option in currentFaction.Options.OrderBy(o => o.SortOrder))
            {
                if (option.Enabled)
                {
                    if (DrawCustomButton(startRect, option.Label, option.TextColor))
                    {
                        option.DoAction(currentFaction, speaker, defendant);
                    }
                    startRect.y += 30;
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawFactionCard(Rect rect, ref int y, FactionInteraction faction)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Rect r = new Rect(10, y, rect.width - 20, 75);
            Rect titleRect = new Rect(15, y, rect.width - 20, 50);
            Widgets.Label(titleRect, faction.Faction.Name);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 22, rect.width - 20, 50);
            FactionRelationKind kindWithPlayer = faction.Faction.PlayerRelationKind;
            Alliance alliance = faction.Alliance;
            Widgets.Label(rect2, "RelationsWithPlayer".Translate(kindWithPlayer.GetLabel(), faction.Faction.PlayerGoodwill.ToStringWithSign(), faction.Trust, "AllianceInformation".Translate(alliance == null ? "NoAllianceAlias".Translate().ToString() : alliance.Name)));

            Text.Font = GameFont.Small;
            Widgets.DrawHighlightIfMouseover(r);

            if (Mouse.IsOver(r))
            {
                string str2 = "CurrentGoodwillTip".Translate();
                if (faction.Faction.def.permanentEnemy)
                {
                    str2 = str2 + "\n\n" + "CurrentGoodwillTip_PermanentEnemy".Translate();
                }
                else
                {
                    str2 += "\n\n";
                    switch (faction.Faction.PlayerRelationKind)
                    {
                        case FactionRelationKind.Ally:
                            str2 += "CurrentGoodwillTip_Ally".Translate(0.ToString("F0"));
                            break;
                        case FactionRelationKind.Neutral:
                            str2 += "CurrentGoodwillTip_Neutral".Translate(0.ToString("F0"), 75.ToString("F0"));
                            break;
                        case FactionRelationKind.Hostile:
                            str2 += "CurrentGoodwillTip_Hostile".Translate(0.ToString("F0"));
                            break;
                    }
                    if (faction.Faction.def.goodwillDailyGain > 0f || faction.Faction.def.goodwillDailyFall > 0f)
                    {
                        str2 = str2 + "\n\n" + "CurrentGoodwillTip_NaturalGoodwill".Translate(faction.Faction.def.naturalColonyGoodwill.min.ToString("F0"), faction.Faction.def.naturalColonyGoodwill.max.ToString("F0"), faction.Faction.def.goodwillDailyGain.ToString("0.#"), faction.Faction.def.goodwillDailyFall.ToString("0.#"));
                    }
                }
                str2 += "TrustFactionInformation".Translate();
                TooltipHandler.TipRegion(r, str2);
            }

            GUI.color = faction.Faction.Color;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;

            var war = hasWarsWithPlayer[faction];
            if (war != null)
            {
                GUI.color = war.WarGoalDef.MenuColor;
                Widgets.DrawBox(r);
                GUI.color = Color.white;

                if (war.Started == false)
                    hasWarsWithPlayer[faction] = null;
            }

            if (Widgets.ButtonInvisible(r))
            {
                currentFaction = faction;
                defendant = currentFaction.Faction.leader;
            }
            y += 85;
        }
        private void DrawAllianceMainPage(Rect inRect)
        {
            Rect rect1 = inRect;
            rect1.width = 500;
            rect1.height = 530;
            Rect rect2 = rect1;
            rect2.x += 500;

            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect1, "DrawAllianceMainPage_AllianceTitle".Translate());
            Widgets.Label(rect2, "DrawAllianceMainPage_CoalitionTitle".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            Rect scrollVertRectFact = new Rect(0, 30, rect1.x, commAllianceSliderLength);
            int y = 60;
            Widgets.BeginScrollView(rect1, ref factionListSlider, scrollVertRectFact, false);
            for (int i = 0; i < alliances.Count; i++)
            {
                Alliance alliance = alliances[i];

                DrawAllianceCard(rect1, ref y, alliance);
            }
            Widgets.EndScrollView();

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect bottomButton = new Rect(inRect.x + 125, 530, 250, 40);
            if(DrawCustomButton(bottomButton, "DrawAllianceMainPage_AllianceCreateButton".Translate(), Color.white))
            {
                if(globalFactionManager.PlayerAlliance == null)
                {
                    Alliance alliance = Alliance.MakeAlliance(NameGenerator.GenerateName(RulePackDefOfLocal.NamerSettlementOutlander), Faction.OfPlayer, AllianceGoalDefOfLocal.CommonGoal);

                    globalFactionManager.AddAlliance(alliance);
                }
                else
                {
                    Messages.Message("Alliance_PlayerAlreadyHaveOne".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
            bottomButton.x += 500;
            if (DrawCustomButton(bottomButton, "DrawAllianceMainPage_CoalitioneCreateButton".Translate(), Color.white))
            {

            }
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            GUI.color = MenuSectionBGBorderColor;
            rect1.height = 556;
            Widgets.DrawBox(rect1);
            GUI.color = Color.white;
        }

        private void DrawAllianceCard(Rect rect, ref int y, Alliance alliance)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Rect r = new Rect(10, y, rect.width - 20, 77);
            Rect titleRect = new Rect(15, y, rect.width - 20, 50);
            Widgets.Label(titleRect, "AllianceNameAllias".Translate(alliance.Name));

            Rect infoRect = new Rect(15, y + 22, rect.width - 20, 55);
            Text.Font = GameFont.Tiny;
            Widgets.Label(infoRect, "DrawAllianceCard_OwnerInfo".Translate(alliance.FactionOwner.Name, alliance.AllianceGoalDef.LabelCap, alliance.Factions.Count));

            if (alliance.PlayerOwner)
            {
                if (Widgets.ButtonInvisible(r))
                {
                    Find.WindowStack.Add(new AllianceManager(alliance, this, speaker));
                }

                GUI.color = MenuSectionBGBorderColor;
                Widgets.DrawBox(r);
                GUI.color = Color.white;
            }

            Widgets.DrawHighlightIfMouseover(r);

            y += 90;
        }
    }
}