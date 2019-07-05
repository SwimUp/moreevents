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

        public List<Quest> Quests;

        public Communications()
        {
            CommunicationDialogs = new Dictionary<string, CommunicationDialog>();
            Quests = new List<Quest>();
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

        public void RemoveQuest(Quest quest, EndCondition condition = EndCondition.None, bool showMessage = true)
        {
            Quests.Remove(quest);

            if(showMessage)
                SendEndQuestMessage(condition, quest);
        }

        public void RemoveQuest(string key, EndCondition condition = EndCondition.None, bool showMessage = true)
        {
            for(int i = 0; i < Quests.Count; i++)
            {
                Quest quest = Quests[i];
                if (quest.UniqueId == key)
                {
                    Quests.Remove(quest);

                    if (showMessage)
                        SendEndQuestMessage(condition, quest);

                    return;
                }
            }
        }

        private void SendEndQuestMessage(EndCondition condition, Quest quest)
        {
            switch(condition)
            {
                case EndCondition.Fail:
                    {
                        Messages.Message("QuestFail".Translate(quest.CardLabel), MessageTypeDefOf.NegativeEvent, true);
                        break;
                    }
                case EndCondition.Success:
                    {
                        Messages.Message("QuestSuccess".Translate(quest.CardLabel), MessageTypeDefOf.NegativeEvent, true);
                        break;
                    }
                case EndCondition.Timeout:
                    {
                        Messages.Message("QuestTimeout".Translate(quest.CardLabel), MessageTypeDefOf.NegativeEvent, true);
                        break;
                    }
            }
        }

        public void AddQuest(Quest quest)
        {
            Quests.Add(quest);
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
            Scribe_Collections.Look(ref Quests, "Quests", LookMode.Deep);
        }
    }
}
