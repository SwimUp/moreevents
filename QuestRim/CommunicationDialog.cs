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
        public string CardLabel;
        public string Description;

        public IncidentDef RelatedIncident;
        public Faction Faction;
        public List<CommOption> Options;

        public void ExposeData()
        {
            Scribe_Values.Look(ref CardLabel, "CardLabel");
            Scribe_Values.Look(ref Description, "Description");

            Scribe_Defs.Look(ref RelatedIncident, "RelatedIncident");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
        }
    }
}
