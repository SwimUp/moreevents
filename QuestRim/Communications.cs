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

        public List<EmailBox> EmailBoxes
        {
            get
            {
                if (emailBoxes == null)
                {
                    emailBoxes = new List<EmailBox>();
                    emailBoxes.Add(new EmailBox()
                    {
                        Owner = Faction.OfPlayer
                    });
                }

                return emailBoxes;
            }
        }
        public EmailBox PlayerBox
        {
            get
            {
                if(playerBox == null)
                {
                    playerBox = EmailBoxes.Where(box => box.Owner == Faction.OfPlayer).FirstOrDefault();
                }

                return playerBox;
            }
        }
        private EmailBox playerBox;

        public List<QuestPawn> QuestPawns
        {
            get
            {
                if(questPawns == null)
                {
                    questPawns = new List<QuestPawn>();
                }

                return questPawns;
            }
        }
        private List<QuestPawn> questPawns;

        private List<CommunicationDialog> communicationDialogs;
        private List<Quest> quests;
        private UniqueIdManager uniqueIdManager;
        private List<CommunicationComponent> components;
        private FactionManager factionManager;
        private List<EmailBox> emailBoxes;

        public Communications()
        {

        }

        public void AddQuestPawn(Pawn pawn, Quest quest)
        {
            pawn.GetQuestPawn(out QuestPawn questPawn);
            if (questPawn != null)
            {
                if (!questPawn.Quests.Contains(quest))
                {
                    questPawn.Quests.Add(quest);
                }
            }
            else
            {
                questPawn = new QuestPawn();
                questPawn.Pawn = pawn;
                questPawn.Quests.Add(quest);

                QuestPawns.Add(questPawn);
            }
        }

        public void RemoveQuestPawn(Pawn pawn)
        {
            pawn.GetQuestPawn(out QuestPawn questPawn);
            if (questPawn != null)
            {
                QuestPawns.Remove(questPawn);
            }
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
            CommunicationDialogs.Remove(dialog);
        }

        public void RemoveQuest(Quest quest, EndCondition condition = EndCondition.None, bool showMessage = true)
        {
            Quests.Remove(quest);

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
                    Quests.Remove(quest);

                    if (showMessage)
                        SendEndQuestMessage(condition, quest);

                    return;
                }
            }
        }

        public void RegisterComponent(CommunicationComponent component)
        {
            Components.Add(component);
        }

        public CommunicationComponent GetComponent(int id)
        {
            for(int i = 0; i < Components.Count; i++)
            {
                CommunicationComponent component = Components[i];
                if(component.id == id)
                {
                    return component;
                }
            }

            return null;
        }

        public void RemoveComponent(int id)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                CommunicationComponent component = components[i];
                if (component.id == id)
                {
                    Components.Remove(component);
                    return;
                }
            }
        }

        public void RemoveComponent(CommunicationComponent component)
        {
            Components.Remove(component);
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

        public void AddQuest(Quest quest, Letter letter = null)
        {
            Quests.Add(quest);

            if(letter != null)
            {
                Find.LetterStack.ReceiveLetter(letter);
            }
        }

        public Letter MakeQuestLetter(Quest quest, string label = null, string description = null, LetterDef letterDef = null, LookTargets lookTarget = null)
        {
            if (letterDef == null)
                letterDef = LetterDefOf.NeutralEvent;

            if (string.IsNullOrEmpty(label))
            {
                label = $"{"QuestPrefix".Translate()}: {quest.CardLabel}";
            }

            if (string.IsNullOrEmpty(description))
                description = quest.Description;

            Letter letter;
            if(lookTarget == null)
                letter = LetterMaker.MakeLetter(label, description, letterDef);
            else
                letter = LetterMaker.MakeLetter(label, description, letterDef, lookTarget);

            return letter;
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
            Scribe_Collections.Look(ref emailBoxes, "EmailBoxes", LookMode.Deep);
            Scribe_Collections.Look(ref questPawns, "QuestPawns", LookMode.Deep);
        }
    }
}
