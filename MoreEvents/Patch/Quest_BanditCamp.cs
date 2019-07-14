using QuestRim;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    public class Quest_BanditCamp : Quest
    {
        public override string CardLabel => "Quest_BanditCamp_CardLabel".Translate();

        public override string Description => questText;
        private string questText;

        public Site QuestSite;

        public Quest_BanditCamp() { }

        public Quest_BanditCamp(string text, Site site)
        {
            questText = text;
            QuestSite = site;
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref questText, "text");
            Scribe_References.Look(ref QuestSite, "QuestSite");
        }
    }
}
