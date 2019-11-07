using RimWorld;
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

        private static readonly Color MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;

        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
        private static Texture2D ChangeNameTexture;

        private Alliance alliance;

        static AllianceManager()
        {
            ChangeNameTexture = ContentFinder<Texture2D>.Get("UI/ChangeAllianceName");
        }

        public AllianceManager(Alliance alliance)
        {
            doCloseX = true;
            forcePause = true;

            this.alliance = alliance;
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
    }
}
