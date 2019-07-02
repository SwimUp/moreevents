using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class Communications : IExposable
    {
        public List<CommunicationDialog> CommunicationDialogs;

        //public List<Quest> Quests;

        public Communications()
        {
            CommunicationDialogs = new List<CommunicationDialog>();
        }

        public void OpenCommunications(Pawn speaker)
        {
            Find.WindowStack.Add(new GeoscapeWindow(this, speaker));
        }

        public void AddCommunication(string cardLabel, string description, Faction faction = null, IncidentDef incident = null, List<CommOption> options = null)
        {
            CommunicationDialog comDialog = new CommunicationDialog
            {
                CardLabel = cardLabel,
                Description = description,
                Faction = faction,
                RelatedIncident = incident,
                Options = options
            };

            CommunicationDialogs.Add(comDialog);
        }

        public void AddCommunication(string cardLabel, string description, Faction faction)
        {
            AddCommunication(cardLabel, description, faction, null);
        }

        public void AddCommunication(string cardLabel, string description)
        {
            AddCommunication(cardLabel, description, null, null, null);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref CommunicationDialogs, "CommunicationDialogs", LookMode.Deep);
        }
    }
}
