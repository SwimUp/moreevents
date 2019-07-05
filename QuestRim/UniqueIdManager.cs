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

        public void ExposeData()
        {
            Scribe_Values.Look(ref nextDialogid, "nextDialogid", 0);
        }
    }
}
