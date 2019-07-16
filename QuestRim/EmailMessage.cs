using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class EmailMessage : IExposable
    {
        public string From;
        public string To;

        public string Subject;

        public string Message;

        public Faction Faction;

        public List<EmailMessageOption> Answers = new List<EmailMessageOption>();

        public void ExposeData()
        {
            Scribe_Values.Look(ref From, "From");
            Scribe_Values.Look(ref To, "To");
            Scribe_Values.Look(ref Subject, "Subject");
            Scribe_Values.Look(ref Message, "Message");

            Scribe_References.Look(ref Faction, "Faction");

            Scribe_Collections.Look(ref Answers, "Answers", LookMode.Deep);
        }
    }
}
