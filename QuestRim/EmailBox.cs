using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class EmailBox : IExposable
    {
        public Faction Owner;

        public List<EmailMessage> Messages = new List<EmailMessage>();

        public void ExposeData()
        {
            Scribe_References.Look(ref Owner, "Owner");
            Scribe_Collections.Look(ref Messages, "Messages", LookMode.Deep);
        }

        public void SendMessage(EmailMessage message, bool notify = true)
        {
            Messages.Add(message);

            if(notify)
            {
                Find.LetterStack.ReceiveLetter("EmailMessageReceivedTitle".Translate(), "EmailMessageReceived".Translate(message.From, message.Subject), LetterDefOf.NeutralEvent);
            }
        }

        public void DeleteMessage(EmailMessage message)
        {
            Messages.Remove(message);
        }

        public EmailMessage FormMessageFrom(Faction faction, string text, string subject)
        {
            EmailMessage message = new EmailMessage();
            message.To = Owner.Name;
            message.From = $"{faction.Name} ({faction.leader.Name})";
            message.Subject = subject;
            message.Message = text;

            return message;
        }
    }
}
