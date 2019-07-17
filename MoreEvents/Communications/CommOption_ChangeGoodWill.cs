using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_ChangeGoodWill : CommOption
    {
        public override string Label => "";

        public int GoodWill;

        public CommOption_ChangeGoodWill()
        {

        }

        public CommOption_ChangeGoodWill(int value)
        {
            GoodWill = value;
        }

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            defendant.Faction.TryAffectGoodwillWith(speaker.Faction, GoodWill);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref GoodWill, "GoodWill");
        }
    }
}
