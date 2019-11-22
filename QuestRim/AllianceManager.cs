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
    public class AllianceManager : Window
    {
        public override Vector2 InitialSize => new Vector2(1000, 700);
        private Vector2 factionSlider = Vector2.zero;
        private Vector2 questSlider = Vector2.zero;
        private Vector2 agreementsSlider = Vector2.zero;

        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;

        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
        private static Texture2D ChangeNameTexture;

        private Alliance alliance;

        public GeoscapeWindow GeoscapeWindow;

        private static readonly Texture2D StorytellerHighlightTex = ContentFinder<Texture2D>.Get("UI/HeroArt/Storytellers/Highlight");

        private List<AllianceAgreementDef> sortedAllianceAgreementsDef;

        public Pawn Negotiator;

        static AllianceManager()
        {
            ChangeNameTexture = ContentFinder<Texture2D>.Get("UI/ChangeAllianceName");
        }

        public AllianceManager(Alliance alliance, GeoscapeWindow window, Pawn negotiator)
        {
            Negotiator = negotiator;

            doCloseX = true;
            forcePause = true;

            this.alliance = alliance;
            GeoscapeWindow = window;

          //  sortedAllianceAgreementsDef = (from def in DefDatabase<AllianceAgreementDef>.AllDefsListForReading orderby def.AgreementCategory, alliance.AgreementActive(def) select def).ToList();
        }
        public override void DoWindowContents(Rect inRect)
        {
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width, 50);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(titleRect, $"{alliance.Name}");
            if(Widgets.ButtonInvisible(titleRect))
            {
                Find.WindowStack.Add(new Dialog_RenameAlliance(alliance));
            }
            Widgets.DrawHighlightIfMouseover(titleRect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect1 = new Rect(inRect.x, inRect.y + 50, inRect.width, inRect.height - 55);

            Rect factionRect = new Rect(rect1.x, rect1.y, 320, rect1.height);
            Rect scrollVertRectFact = new Rect(0, 0, factionRect.x, alliance.Factions.Count * 85);
            int y = 10;
            Widgets.BeginScrollView(factionRect, ref factionSlider, scrollVertRectFact, false);
            for (int i = 0; i < alliance.Factions.Count; i++)
            {
                FactionInteraction faction = alliance.Factions[i];

                DrawFactionCard(factionRect, ref y, faction);
            }
            Widgets.EndScrollView();

            GUI.color = MenuSectionBGBorderColor;
            Widgets.DrawBox(factionRect);
            Widgets.DrawLineHorizontal(0, 50, inRect.width);
            GUI.color = Color.white;

            Rect rect2 = new Rect(330, inRect.y + 50, inRect.width - 330, inRect.height - 55);
            DrawRightBlock(rect2);
        }

        private void DrawRightBlock(Rect inRect)
        {
            Rect rect = inRect;
            rect.y += 10;
            rect.height = 50;
            Widgets.Label(rect, "AllianceManager_Goal".Translate(alliance.AllianceGoalDef.LabelCap, alliance.AllianceGoalDef.description));
            rect.y += 60;

            Rect questBoxInfoRect = new Rect(rect.x, rect.y, rect.width, 250);
            DrawQuestBox(questBoxInfoRect);

            rect.y += 250;

            Rect agreementsInfoRect = new Rect(rect.x, rect.y, rect.width, 200);
            DrawAgreements(agreementsInfoRect);
        }

        private void DrawFactionCard(Rect rect, ref int y, FactionInteraction faction)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Rect r = new Rect(10, y, rect.width - 20, 70);
            Rect titleRect = new Rect(15, y, rect.width - 20, 50);
            Widgets.Label(titleRect, faction.Faction.Name);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 22, rect.width - 30, 20);
            Widgets.Label(rect2, "AllianceManager_TrustFactionLevel".Translate(faction.Trust));
            Text.Anchor = TextAnchor.MiddleCenter;
            rect2.y += 20;
            if(GUIUtils.DrawCustomButton(rect2, "AllianceManager_KickButton".Translate(), Color.white))
            {
                alliance.RemoveFaction(faction, AllianceRemoveReason.Kick);
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            GUI.color = faction.Faction.Color;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;

            y += 80;
        }

        private void DrawAgreements(Rect rect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 28), "AllianceManager_ActiveAgreements".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            float x = 0;
            float width = rect.width - 20;
            Rect agreementListRect = new Rect(rect.x, rect.y + 33, width, 160);
            Rect scrollVertRectFact = new Rect(0, 0, DefDatabase<AllianceAgreementDef>.DefCount * 140, 135);
            Rect agreementRect = new Rect(0, 0, 130, 145);
            Widgets.BeginScrollView(agreementListRect, ref agreementsSlider, scrollVertRectFact, true);
            foreach (var agreement in from def in DefDatabase<AllianceAgreementDef>.AllDefsListForReading orderby def.AgreementCategory, alliance.AgreementActive(def) select def)
            {
                DrawAgreement(agreementRect, ref x, agreement);
            }
            Widgets.EndScrollView();
            GUIUtils.DrawLineHorizontal(rect.x, rect.y + 195, rect.width, MenuSectionBGBorderColor);
        }

        private void DrawAgreement(Rect rect, ref float x, AllianceAgreementDef allianceAgreementDef)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect textureRect = new Rect(x + 25, 2, 80, 80);

            GUI.DrawTexture(textureRect, alliance.AgreementActive(allianceAgreementDef) ? allianceAgreementDef.AgreementMenuTexture : allianceAgreementDef.OfflineAgreementMenuTexture);

            Rect titleRect = new Rect(x + 3, 85, 124, 55);
            Widgets.Label(titleRect, allianceAgreementDef.label);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect buttonRect = new Rect(x, 0, 130, 140);
            if (Widgets.ButtonInvisible(buttonRect))
            {
                var comp = allianceAgreementDef.Comp;
                if(comp.CanSign(alliance, allianceAgreementDef, Negotiator, out string reason))
                {
                    comp.MenuSelect(alliance, allianceAgreementDef, Negotiator);
                }
            }
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(buttonRect);
            if (Mouse.IsOver(buttonRect))
            {
                string info = allianceAgreementDef.description;
                if (!allianceAgreementDef.Comp.CanSign(alliance, allianceAgreementDef, Negotiator, out string reason))
                {
                    info += "\n" + reason;
                }
                TooltipHandler.TipRegion(buttonRect, info);
            }

            x += 140;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawQuestBox(Rect rect)
        {
            float y = rect.y;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 28), "AllianceManager_AvaliableQuests".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            y += 33;

            float width = rect.width - 20;
            Rect questListRect = new Rect(rect.x, y, width, 200);
            Rect scrollVertRectFact = new Rect(0, 0, width - 20, alliance.AllianceQuests.Count() * 35);
            y = 0;
            Rect questRect = new Rect(0, y, width - 20, 120);
            Widgets.BeginScrollView(questListRect, ref questSlider, scrollVertRectFact, true);
            foreach(var quest in alliance.AllianceQuests)
            {
                DrawQuest(questRect, ref y, quest);
            }
            Widgets.EndScrollView();
            y = rect.y + 240;
            GUIUtils.DrawLineHorizontal(rect.x, y, rect.width, MenuSectionBGBorderColor);
        }

        private void DrawQuest(Rect rect, ref float y, Quest quest)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            Rect titleRect = new Rect(0, y, rect.width, 27);
            string questLabel = quest.CardLabel.Length > 36 ? quest.CardLabel.Substring(0, 36) : quest.CardLabel;
            Widgets.Label(titleRect, "AllianceManager_QuestLabel".Translate(questLabel, quest.Faction.Name));
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (Widgets.ButtonInvisible(titleRect))
            {
                GeoscapeWindow.ForceSelectQuest(quest);

                Close();
            }
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            Widgets.DrawHighlightIfMouseover(titleRect);

            GUI.color = quest.Faction.Color;
            Widgets.DrawHighlight(titleRect);
            GUI.color = Color.white;
            y += 35;

            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
