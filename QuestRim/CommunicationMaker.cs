using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public static class CommunicationMaker
    {
        public static CommunicationDialog MakeDialog(string cardLabel, string description, Faction faction = null, IncidentDef incident = null, List<CommOption> options = null)
        {
            return new CommunicationDialog(QuestsManager.Communications.UniqueIdManager.GetNextDialogID(), cardLabel, description, incident, faction, options);
        }
    }
}
