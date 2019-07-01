using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class CommunicationDialog : IExposable
    {
        public CommunicationDialogDef CommDef;
        public Faction Faction;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref CommDef, "CommDef");
            Scribe_References.Look(ref Faction, "Faction");
        }
    }
}
