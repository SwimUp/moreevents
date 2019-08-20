using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_BadJoke : EmailMessageOption
    {
        public override string Label => "BadJoke".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            message.Faction.TryAffectGoodwillWith(box.Owner, -10);

            box.DeleteMessage(message);
        }
    }
}
