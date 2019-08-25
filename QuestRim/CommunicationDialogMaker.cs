using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public static class CommunicationDialogMaker
    {
        public static CommunicationDialog MakeCommunicationDialog(string cardLabel, string description, Faction faction, IncidentDef incident, List<CommOption> options)
        {
            CommunicationDialog comDialog = new CommunicationDialog
            {
                id = QuestsManager.Communications.UniqueIdManager.GetNextDialogID(),
                CardLabel = cardLabel,
                Description = description,
                Faction = faction,
                RelatedIncident = incident,
                Options = options
            };

            return comDialog;
        }
    }
}
