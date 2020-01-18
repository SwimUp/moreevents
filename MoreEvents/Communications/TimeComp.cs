using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class TimeComp : CommunicationComponent
    {
        public CommunicationDialog Dialog;

        public string EmailMessageSubject;
        public Faction EmailBoxOwner; 

        public int TicksToRemove = 0;

        public CommOption Action;
        public Pawn Speaker;
        public Pawn Defendant;

        public Letter EndLetter;

        public TimeComp()
        {

        }

        public TimeComp(CommunicationDialog dialog, int ticks)
        {
            Dialog = dialog;
            TicksToRemove = ticks;
        }

        public TimeComp(string messageSubject, Faction boxOwner, int ticks)
        {
            EmailMessageSubject = messageSubject;
            EmailBoxOwner = boxOwner;
            TicksToRemove = ticks;
        }

        public override void Tick()
        {
            TicksToRemove--;

            if(TicksToRemove <= 0)
            {
                RemoveNow();
            }
        }

        public virtual void RemoveNow()
        {
            try
            {
                if (Dialog != null)
                {
                    Dialog.Destroy();
                }

                if (!string.IsNullOrEmpty(EmailMessageSubject) && EmailBoxOwner != null)
                {
                    EmailBox box = QuestsManager.Communications.EmailBoxes.Where(b => b.Owner == EmailBoxOwner).FirstOrDefault();
                    if (box != null)
                    {
                        EmailMessage message = box.Messages.Where(m => m.Subject == EmailMessageSubject).FirstOrDefault();
                        if (message != null)
                        {
                            box.Messages.Remove(message);
                        }
                    }
                }

                if (EndLetter != null)
                {
                    Find.LetterStack.ReceiveLetter(EndLetter);
                }

                if (Action != null)
                {
                    Action.DoAction(null, Speaker, Defendant);
                }
            }catch(Exception ex)
            {
                Log.Error($"Error while removing component {id} --> TimeComp: {ex}");
            }

            QuestsManager.Communications.RemoveComponent(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref Dialog, "Dialog");
            Scribe_Values.Look(ref TicksToRemove, "TicksToRemove");
            Scribe_Values.Look(ref EmailMessageSubject, "EmailMessageSubject");
            Scribe_References.Look(ref EmailBoxOwner, "EmailBoxOwner");
            Scribe_Deep.Look(ref Action, "Action");
            Scribe_Deep.Look(ref EndLetter, "EndLetter");
            Scribe_References.Look(ref Speaker, "Speaker");
            Scribe_References.Look(ref Defendant, "Defendant");
        }
    }
}
