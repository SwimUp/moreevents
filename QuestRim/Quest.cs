using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class Quest : IExposable
    {
        public abstract string CardLabel { get; }
        public abstract string Description { get; }

        public Faction Faction;
        public List<CommOption> Options;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
        }
    }
}
