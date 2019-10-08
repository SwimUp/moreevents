using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class DarkNETWindow_Quests : Window
    {
        private Vector2 commSlider = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(950, 674);

        public List<Quest> Quests;

        public DarkNETWindow_Quests(List<Quest> quests)
        {
            forcePause = true;
            doCloseX = true;

            Quests = quests;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect traderLabelOnlineRect = new Rect(0, 0, 950, 30);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(traderLabelOnlineRect, "DarkNET_QuestsHeaders".Translate(Quests.Count));
            Text.Anchor = TextAnchor.UpperLeft;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect questsList = new Rect(10, 35, 900, 600);
            Rect questRect = new Rect(0, 10, 884, 210);
            Rect scrollVertRectFact = new Rect(0, 0, questsList.x, Quests.Count * 222);

            Widgets.BeginScrollView(questsList, ref commSlider, scrollVertRectFact, true);
            for(int i = 0; i < Quests.Count; i++)
            {
                Quest quest = Quests[i];

                GUIUtils.DrawQuestCard(quest, Quests, questRect);
                questRect.y += 220;
            }
            Widgets.EndScrollView();
        }
    }
}
