using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class WarManager : Window
    {
        public override Vector2 InitialSize => new Vector2(1000, 700);

        private Alliance alliance;

        private Vector2 warsSlider = Vector2.zero;
        private Vector2 defendersSlider = Vector2.zero;

        private Vector2 statWorkerSlider = Vector2.zero;

        private List<War> wars;
        private War currentWar;

        private string stat;

        public WarManager()
        {
            doCloseX = true;
            forcePause = true;
        }

        public WarManager(Alliance alliance)
        {
            doCloseX = true;
            forcePause = true;

            this.alliance = alliance;

            wars = alliance.Wars.ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect warRect = new Rect(inRect.x, inRect.y, 400, inRect.height);
            Rect scrollVertRectFact = new Rect(0, 0, warRect.x, wars.Count * 85);
            int y = 10;
            Widgets.BeginScrollView(warRect, ref warsSlider, scrollVertRectFact, false);
            for (int i = 0; i < wars.Count; i++)
            {
                War war = wars[i];

                DrawWarRect(warRect, ref y, war);
            }
            Widgets.EndScrollView();

            GUI.color = GUIUtils.MenuSectionBGBorderColor;
            Widgets.DrawBox(warRect);
            Widgets.DrawLineHorizontal(0, 0, inRect.width);
            GUI.color = Color.white;

            if (currentWar != null)
            {
                Rect mainInfoWarRect = new Rect(inRect.x + 410, inRect.y, inRect.width - 420, inRect.height);

                DrawWarInfo(mainInfoWarRect);
            }
        }

        private void DrawWarInfo(Rect rect)
        {
            float y = rect.y;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect labelRect = new Rect(rect.x, y, rect.width, 35);
            Widgets.Label(labelRect, $"{currentWar.WarName} ({currentWar.WarGoalDef.LabelCap})");
            Text.Font = GameFont.Small;
            GUIUtils.DrawLineHorizontal(rect.x, y + 30, rect.width, GUIUtils.CommBorderColor);

            labelRect.y += 50;
            y += 50;
            Widgets.Label(labelRect, "WarManager_DrawWarInfo_WarMembers".Translate(currentWar.DefendAlliance == null ? "WarManager_NoAlliance".Translate().ToString() : currentWar.DefendAlliance.Name));
            y += 30;
            Rect defenderRect = new Rect(rect.x, y, rect.width, 250);
            Rect scrollVertRectFact = new Rect(0, 0, rect.x, currentWar.DefendingFactions.Count * 35);
            Widgets.BeginScrollView(defenderRect, ref defendersSlider, scrollVertRectFact, true);
            Rect factionRect = new Rect(0, 10, rect.width - 10, 30);
            for (int i = 0; i < currentWar.DefendingFactions.Count; i++)
            {
                FactionInteraction faction = currentWar.DefendingFactions[i];

                GUI.color = faction.Faction.Color;
                Widgets.DrawHighlight(factionRect);
                GUI.color = Color.white;

                Widgets.Label(factionRect, faction.Faction.Name);

                factionRect.y += 35;
                
            }
            Widgets.EndScrollView();

            y += 255;

            GUIUtils.DrawLineHorizontal(rect.x, y, rect.width, GUIUtils.CommBorderColor);

            y += 5;

            Rect statRect = new Rect(rect.x, y, rect.width, 200);
            Widgets.LabelScrollable(statRect, stat, ref statWorkerSlider);

            y += 205;
            GUIUtils.DrawLineHorizontal(rect.x, y, rect.width, GUIUtils.CommBorderColor);

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawWarRect(Rect rect, ref int y, War war)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Rect r = new Rect(10, y, rect.width - 20, 70);
            Rect titleRect = new Rect(15, y, rect.width - 20, 50);
            Widgets.Label(titleRect, war.WarName);
            Text.Font = GameFont.Tiny;
            Rect rect2 = new Rect(15, y + 22, rect.width - 30, 50);
            Widgets.Label(rect2, "WarManager_Factions".Translate(war.WarGoalDef.LabelCap, war.AssaultFactions.Count, war.AttackedAlliance == null ? "WarManager_NoAlliance".Translate().ToString() : war.AttackedAlliance.Name, war.DefendingFactions.Count,
                 war.DefendAlliance == null ? "WarManager_NoAlliance".Translate().ToString() : war.DefendAlliance.Name));

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            if(Widgets.ButtonInvisible(r))
            {
                if (war != currentWar)
                {
                    currentWar = war;
                    stat = currentWar.StatWorker.GetStat();
                }
            }

            GUI.color = war.WarGoalDef.MenuColor;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;

            Widgets.DrawHighlightIfMouseover(r);

            y += 85;
        }
    }
}
