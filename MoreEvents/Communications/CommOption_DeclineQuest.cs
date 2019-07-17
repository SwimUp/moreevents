using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_DeclineQuest : CommOption
    {
        public override string Label => "CommOption_DeclineQuest_Label".Translate();

        public Quest Quest;
        public EndCondition EndCondition;

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if(Quest != null)
            {
                QuestsManager.Communications.RemoveQuest(Quest, EndCondition);
                if(defendant.GetQuestPawn(out QuestPawn questPawn))
                {
                    if(questPawn.Quests.Contains(Quest))
                    {
                        questPawn.Quests.Remove(Quest);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref EndCondition, "EndCondition");
            Scribe_References.Look(ref Quest, "Quest");
        }
    }
}
