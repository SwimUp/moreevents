using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Dialogs
{
    public class EmailMessageOption_DeclineQuest_Laboratory : EmailMessageOption
    {
        public override string Label => "EmailMessageOption_DeclineQuest_Laboratory".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            Find.LetterStack.ReceiveLetter("EmailMessageOption_DeclineQuest_LaboratoryTitle".Translate(), "EmailMessageOption_DeclineQuest_LaboratoryDesc".Translate(), LetterDefOf.NeutralEvent);

            box.DeleteMessage(message);
        }
    }
}
