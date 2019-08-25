using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class CommunicationComponent_CommunicationDialogTimer : CommunicationComponent_Timer
    {
        private CommunicationDialog dialog;

        private string notification;

        public CommunicationComponent_CommunicationDialogTimer()
        {

        }

        public CommunicationComponent_CommunicationDialogTimer(int ticks, CommunicationDialog dialog, string notification = null) : base(ticks)
        {
            this.dialog = dialog;
            this.notification = notification;
        }

        public override void TimerEnd()
        {
            QuestsManager.Communications.AddCommunication(dialog);

            Find.LetterStack.ReceiveLetter("CommunicationComponent_CommunicationDialogTimer".Translate(),
                notification, LetterDefOf.NeutralEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref dialog, "dialog");
            Scribe_Values.Look(ref notification, "notification");
        }
    }
}
