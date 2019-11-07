using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class UniqueIdManager : IExposable
    {
        private int nextDialogid;
        private int nextQuestId;
        private int nextComponentId;
        private int nextAllianceId;
        private int nextFactionInteractionId;

        private static int GetNextID(ref int nextID)
        {
            if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Warning("Getting next unique ID during saving or loading. This may cause bugs.");
            }
            int result = nextID;
            nextID++;
            if (nextID == int.MaxValue)
            {
                Log.Warning("Next ID is at max value. Resetting to 0. This may cause bugs.");
                nextID = 0;
            }
            return result;
        }
        public int GetNextDialogID()
        {
            return GetNextID(ref nextDialogid);
        }

        public int GetNextQuestID()
        {
            return GetNextID(ref nextQuestId);
        }

        public int GetNextComponentID()
        {
            return GetNextID(ref nextComponentId);
        }

        public int GetNextAllianceID()
        {
            return GetNextID(ref nextAllianceId);
        }

        public int GetNextFactionInteractionId()
        {
            return GetNextID(ref nextFactionInteractionId);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref nextDialogid, "nextDialogid", 0);
            Scribe_Values.Look(ref nextQuestId, "nextQuestId", 0);
            Scribe_Values.Look(ref nextComponentId, "nextComponentId", 0);
            Scribe_Values.Look(ref nextAllianceId, "nextAllianceId", 0);
            Scribe_Values.Look(ref nextFactionInteractionId, "nextFactionInteractionId");
        }
    }
}
