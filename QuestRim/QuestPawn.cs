﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class QuestPawn : IExposable
    {
        public Pawn Pawn;
        public List<Quest> Quests = new List<Quest>();

        public bool WorldQuester = false;

        public void ShowQuestDialog(Pawn speaker)
        {
            Find.WindowStack.Add(new Dialog_QuestDialog(this, speaker));
        }

        public void Destroy()
        {
            for(int i = 0; i < Quests.Count; i++)
            {
                Quest quest = Quests[i];
                if(quest.Site != null)
                {
                    quest.Site.EndQuest(null, EndCondition.None);
                }
                else
                {
                    quest.EndQuest(null, EndCondition.None);
                }
            }

            QuestsManager.Communications.RemoveQuestPawn(this);
        }
        public void ExposeData()
        {
            Scribe_References.Look(ref Pawn, "Pawn");
            Scribe_Collections.Look(ref Quests, "Quests", LookMode.Reference);
            Scribe_Values.Look(ref WorldQuester, "WorldQuester");
        }
    }
}
