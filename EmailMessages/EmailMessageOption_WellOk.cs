using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_WellOk : EmailMessageOption
    {
        public override string Label => "WellOK".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            Utils.SendRaid(message.Faction, 1.2f, 3 * 60000);

            box.DeleteMessage(message);
        }
    }
}
