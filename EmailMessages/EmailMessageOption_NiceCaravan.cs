using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_NiceCaravan : EmailMessageOption
    {
        public override string Label => "NiceCaravan".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            EmailMessage msg = box.FormMessageFrom(message.Faction, "NiceCaravan_Explain".Translate(), "NiceCaravan_Subject".Translate());

            box.SendMessage(msg);

            message.Faction.TryAffectGoodwillWith(box.Owner, -15);

            Utils.SendRaid(message.Faction, 1.5f, 50000);
            Utils.SendRaid(message.Faction, 1.5f, 2 * 60000);
        }
    }
}
