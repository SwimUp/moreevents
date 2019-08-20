using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_Wtf : EmailMessageOption
    {
        public override string Label => "Wtf".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            EmailMessage msg = box.FormMessageFrom(message.Faction, "Wtf_Explain".Translate(), "Wtf_Subject".Translate());

            box.SendMessage(msg);
        }
    }
}
