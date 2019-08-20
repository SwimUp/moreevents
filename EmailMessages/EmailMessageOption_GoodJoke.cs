using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_GoodJoke : EmailMessageOption
    {
        public override string Label => "GoodJoke".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            message.Faction.TryAffectGoodwillWith(box.Owner, 1);

            box.DeleteMessage(message);
        }
    }
}
