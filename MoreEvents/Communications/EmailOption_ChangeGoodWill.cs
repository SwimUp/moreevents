using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class EmailOption_ChangeGoodWill : EmailMessageOption
    {
        public override string Label => "";

        public int GoodWill;

        public EmailOption_ChangeGoodWill()
        {

        }

        public EmailOption_ChangeGoodWill(int value)
        {
            GoodWill = value;
        }

        public override void DoAction(EmailMessage emailMessage, EmailBox box, Pawn pawn)
        {
            pawn.Faction.TryAffectGoodwillWith(box.Owner, GoodWill);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref GoodWill, "GoodWill");
        }
    }
}
