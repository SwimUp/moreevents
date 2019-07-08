using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Quests
{
    public class Quest_BuildNewBase : Quest
    {
        public override string AdditionalQuestContentString => "HowToGivePawns".Translate();

        public override string CardLabel => "Quest_BuildNewBase_Label".Translate();

        public override string Description => "Quest_BuildNewBase_Description".Translate(Faction);

        public override string PlaceLabel => "Quest_BuildNewBase_Place".Translate(Faction);

        public override string ExpandingIconPath => "Quests/Quest_BuildNewBase";

        public int PawnsRequired;
        public int TicksToEnd;

    }
}
