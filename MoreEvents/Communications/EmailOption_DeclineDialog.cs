using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class EmailOption_DeclineDialog : EmailMessageOption
    {
        public override string Label => "CommOption_DeclineDialog_Label".Translate();

        public EmailMessageOption ExecuteOption;

        public EmailOption_DeclineDialog()
        {

        }

        public EmailOption_DeclineDialog(EmailMessageOption executeOption)
        {
            ExecuteOption = executeOption;
        }

        public override void DoAction(EmailMessage emailMessage, EmailBox box, Pawn pawn)
        {
            ExecuteOption.DoAction(emailMessage, box, pawn);

            box.Messages.Remove(emailMessage);
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref ExecuteOption, "ExecuteOption");
        }
    }
}
