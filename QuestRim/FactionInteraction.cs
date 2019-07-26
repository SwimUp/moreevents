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

        public T GetOption<T>() where T : InteractionOption
        {
            for(int i = 0; i < Options.Count; i++)
            {
                if (Options[i] is T val)
                {
                    return val;
                }
            }

            return null;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
        }
    }
}
