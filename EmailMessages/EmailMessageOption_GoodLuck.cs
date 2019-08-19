using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_GoodLuck : EmailMessageOption
    {
        public override string Label => "WishGoodLuck".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            box.DeleteMessage(message);
        }
    }
}
