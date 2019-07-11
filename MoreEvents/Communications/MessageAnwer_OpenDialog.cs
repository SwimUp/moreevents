using DiaRim;
using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class MessageAnwer_OpenDialog : EmailMessageOption
    {
        public override string Label => labelInt;
        public DialogDef Dialog;
        private string labelInt;

        public MessageAnwer_OpenDialog()
        {

        }

        public MessageAnwer_OpenDialog(string buttonLabel, DialogDef dialog)
        {
            Dialog = dialog;
            labelInt = buttonLabel;
        }

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            throw new NotImplementedException();
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref Dialog, "Dialog");
            Scribe_Values.Look(ref labelInt, "buttonLabel");
        }
    }
}
