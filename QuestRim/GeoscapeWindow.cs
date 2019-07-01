using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class GeoscapeWindow : Window
    {
        private Vector2 commSlider = Vector2.zero;
        private Vector2 commInfoSlider = Vector2.zero;

        private enum Tab
        {
            Quests,
            Events,
            Interaction
        }

        public override Vector2 InitialSize => new Vector2(1000, 700);

        private Tab tab;

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private List<CommunicationDialog> communicationsDialogs => communications.CommunicationDialogs;
        private Communications communications;
        private Pawn speaker;
        private int commDialogSliderLength = 0;
        private CommunicationDialog currentDialog;

        private static readonly Color DisabledSkillColor = new Color(1f, 1f, 1f, 0.5f);

        private static Texture2D PassionMinorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinor");

        private static Texture2D PassionMajorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajor");

        private static Texture2D SkillBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));

        public GeoscapeWindow(Communications communications, Pawn speaker)
        {
            this.communications = communications;
            this.speaker = speaker;

            commDialogSliderLength = communicationsDialogs.Count * 70;

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
            }, tab == Tab.Events));
            tabsList.Add(new TabRecord("QuestsCommTab".Translate(), delegate
            {
                tab = Tab.Quests;
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
            rect1.xMax = 325;
            rect1.height = 500;
            Widgets.DrawBox(rect1);
            Rect scrollVertRectFact = new Rect(0, 0, rect1.x, commDialogSliderLength);
            int y = 0;
            Widgets.BeginScrollView(rect1, ref commSlider, scrollVertRectFact, false);
            GUI.BeginGroup(rect1);
            foreach(var comDialog in communicationsDialogs)
            {
                DrawCommCard(rect1, ref y, comDialog);
            }
            GUI.EndGroup();
            Widgets.EndScrollView();

            DrawPawnCard();

            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(335, rect2.yMin + 20, 610, rect2.yMax);
            GUI.BeginGroup(rect3);
            if(currentDialog != null)
            {
                Widgets.LabelScrollable(new Rect(0, 0, 610, rect3.yMax), currentDialog.CommDef.description, ref commInfoSlider, false, false);
            }
            GUI.EndGroup();
        }

        private void DrawPawnCard()
        {
            GUI.DrawTexture(new Rect(0, 530, 100, 140), PortraitsCache.Get(speaker, new Vector2(100, 140)));
            Widgets.Label(new Rect(100, 550, 100, 30), speaker.Name.ToStringFull);
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
            Widgets.Label(r, dialog.CommDef.label);
            Widgets.DrawHighlightIfMouseover(r);
            if(Widgets.ButtonInvisible(r))
            {
                currentDialog = dialog;
            }
            y += 75;
        }

        private void QuestsPage(Rect inRect)
        {

        }

        private void InteractionPage(Rect inRect)
        {

        }
    }
}
