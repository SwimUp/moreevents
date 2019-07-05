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

        public List<Quest> Quests;

        public UniqueIdManager UniqueIdManager;

        public Communications()
        {
            CommunicationDialogs = new List<CommunicationDialog>();
            Quests = new List<Quest>();
            UniqueIdManager = new UniqueIdManager();
        }

        public void OpenCommunications(Pawn speaker)
        {
            Find.WindowStack.Add(new GeoscapeWindow(this, speaker));
        }

        public void RemoveCommunication(int key)
        {
            for (int i = 0; i < CommunicationDialogs.Count; i++)
            {
                CommunicationDialog dialog = CommunicationDialogs[i];
                if (dialog.id == key)
                {
                    CommunicationDialogs.Remove(dialog);
                    return;
                }
            }
        }

        public void RemoveCommunication(CommunicationDialog dialog)
        {
            CommunicationDialogs.Remove(dialog);
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

        public CommunicationDialog AddCommunication(int id, string cardLabel, string description, Faction faction = null, IncidentDef incident = null, List<CommOption> options = null)
        {
            CommunicationDialog comDialog = new CommunicationDialog
            {
                id = id,
                CardLabel = cardLabel,
                Description = description,
                Faction = faction,
                RelatedIncident = incident,
                Options = options
            };

            CommunicationDialogs.Add(comDialog);

            return comDialog;
        }

        public CommunicationDialog AddCommunication(int id, string cardLabel, string description, Faction faction)
        {
            return AddCommunication(id, cardLabel, description, faction, null);
        }

        public CommunicationDialog AddCommunication(int id, string cardLabel, string description)
        {
            return AddCommunication(id, cardLabel, description, null, null, null);
        }

        public void AddCommunication(CommunicationDialog dialog)
        {
            CommunicationDialogs.Add(dialog);
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref UniqueIdManager, "UniqueIdManager");
            Scribe_Collections.Look(ref CommunicationDialogs, "CommunicationDialogs", LookMode.Deep);
            Scribe_Collections.Look(ref Quests, "Quests", LookMode.Deep);
        }
    }
}
