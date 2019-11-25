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

        private List<War> wars;
        private War currentWar;

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
            Widgets.Label(rect2, "WarManager_Factions".Translate(war.WarGoalDef.LabelCap, war.AssaultFactions.Count, war.AttackedAlliance == null ? "WarManager_NoAlliance".Translate() : war.AttackedAlliance.Name, war.DefendingFactions.Count,
                 war.DefendAlliance == null ? "WarManager_NoAlliance".Translate() : war.DefendAlliance.Name));

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            if(Widgets.ButtonInvisible(r))
            {
                if(war != currentWar)
                    currentWar = war;
            }

            GUI.color = war.WarGoalDef.MenuColor;
            Widgets.DrawHighlight(r);
            GUI.color = Color.white;

            y += 85;
        }
    }
}
