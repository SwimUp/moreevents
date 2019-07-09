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
        public List<CommunicationDialog> CommunicationDialogs
        {
            get
            {
                if (communicationDialogs == null)
                {
                    communicationDialogs = new List<CommunicationDialog>();
                }

                return communicationDialogs;
            }
        }

        public List<Quest> Quests
        {
            get
            {
                if(quests == null)
                {
                    quests = new List<Quest>();
                }

                return quests;
            }
        }

        public UniqueIdManager UniqueIdManager
        {
            get
            {
                if(uniqueIdManager == null)
                {
                    uniqueIdManager = new UniqueIdManager();
                }

                return uniqueIdManager;
            }
        }

        public List<CommunicationComponent> Components
        {
            get
            {
                if (components == null)
                {
                    components = new List<CommunicationComponent>();
                }

                return components;
            }
        }

        public FactionManager FactionManager
        {
            get
            {
                if (factionManager == null)
                {
                    factionManager = new FactionManager();
                }

                return factionManager;
            }
        }

        private List<CommunicationDialog> communicationDialogs;
        private List<Quest> quests;
        private UniqueIdManager uniqueIdManager;
        private List<CommunicationComponent> components;
        private FactionManager factionManager;

        public Communications()
        {
            communicationDialogs = new List<CommunicationDialog>();
            quests = new List<Quest>();
            uniqueIdManager = new UniqueIdManager();
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
                    communicationDialogs.Remove(dialog);
                    return;
                }
            }
        }

        public void RemoveCommunication(CommunicationDialog dialog)
        {
            communicationDialogs.Remove(dialog);
        }

        public void RemoveQuest(Quest quest, EndCondition condition = EndCondition.None, bool showMessage = true)
        {
            quests.Remove(quest);

            if(showMessage)
                SendEndQuestMessage(condition, quest);
        }

        public void RemoveQuest(int key, EndCondition condition = EndCondition.None, bool showMessage = true)
        {
            for(int i = 0; i < Quests.Count; i++)
            {
                Quest quest = Quests[i];
                if (quest.id == key)
                {
                    quests.Remove(quest);

                    if (showMessage)
                        SendEndQuestMessage(condition, quest);

                    return;
                }
            }
        }

        public void RegisterComponent(CommunicationComponent component)
        {
            components.Add(component);
        }

        public CommunicationComponent GetComponent(int id)
        {
            for(int i = 0; i < components.Count; i++)
            {
                CommunicationComponent component = components[i];
                if(component.id == id)
                {
                    return component;
                }
            }

            return null;
        }

        public void RemoveComponent(int id)
        {
            for (int i = 0; i < components.Count; i++)
            {
                CommunicationComponent component = components[i];
                if (component.id == id)
                {
                    components.Remove(component);
                    return;
                }
            }
        }

        public void RemoveComponent(CommunicationComponent component)
        {
            components.Remove(component);
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
            Scribe_Deep.Look(ref uniqueIdManager, "UniqueIdManager");
            Scribe_Deep.Look(ref factionManager, "FactionManager");
            Scribe_Collections.Look(ref communicationDialogs, "CommunicationDialogs", LookMode.Deep);
            Scribe_Collections.Look(ref quests, "Quests", LookMode.Deep);
            Scribe_Collections.Look(ref components, "components", LookMode.Deep);
        }
    }
}
