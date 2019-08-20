using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_JustGetABullet : EmailMessageOption
    {
        public override string Label => "JustGetABullet".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            Utils.SendRaid(message.Faction, 1.4f, 30000);

            box.DeleteMessage(message);
        }
    }
}
