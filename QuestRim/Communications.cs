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
        public Dictionary<string, CommunicationDialog> CommunicationDialogs;

        //public List<Quest> Quests;

        public Communications()
        {
            CommunicationDialogs = new Dictionary<string, CommunicationDialog>();
        }

        public void OpenCommunications(Pawn speaker)
        {
            Find.WindowStack.Add(new GeoscapeWindow(this, speaker));
        }

        public void RemoveCommunication(string id)
        {
            if(CommunicationDialogs.ContainsKey(id))
            {
                CommunicationDialogs.Remove(id);
            }
        }

        public void AddCommunication(string uniqueId, string cardLabel, string description, Faction faction = null, IncidentDef incident = null, List<CommOption> options = null)
        {
            CommunicationDialog comDialog = new CommunicationDialog
            {
                CardLabel = cardLabel,
                Description = description,
                Faction = faction,
                RelatedIncident = incident,
                Options = options
            };

            CommunicationDialogs.Add(uniqueId, comDialog);
        }

        public void AddCommunication(string uniqueId, string cardLabel, string description, Faction faction)
        {
            AddCommunication(uniqueId, cardLabel, description, faction, null);
        }

        public void AddCommunication(string uniqueId, string cardLabel, string description)
        {
            AddCommunication(uniqueId, cardLabel, description, null, null, null);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref CommunicationDialogs, "CommunicationDialogs", LookMode.Value, LookMode.Deep);
        }
    }
}
