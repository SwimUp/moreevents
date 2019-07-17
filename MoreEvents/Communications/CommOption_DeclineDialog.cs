using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_DeclineDialog : CommOption
    {
        public override string Label => "CommOption_DeclineDialog_Label".Translate();

        public CommOption ExecuteOption;

        public CommOption_DeclineDialog()
        {

        }

        public CommOption_DeclineDialog(CommOption executeOption)
        {
            ExecuteOption = executeOption;
        }

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            if (QuestsManager.Communications.CommunicationDialogs.Contains(dialog))
            {
                QuestsManager.Communications.RemoveCommunication(dialog);
            }

            if (defendant.GetQuestPawn(out QuestPawn questPawn))
            {
                if (questPawn.Dialogs.Contains(dialog))
                {
                    questPawn.Dialogs.Remove(dialog);
                }
            }

            ExecuteOption.DoAction(dialog, speaker, defendant);
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref ExecuteOption, "ExecuteOption");
        }
    }
}
