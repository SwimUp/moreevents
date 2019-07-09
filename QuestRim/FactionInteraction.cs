using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class FactionInteraction : IExposable
    {
        public Faction Faction;
        public List<InteractionOption> Options;

        public void ExposeData()
        {
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
        }
    }
}
