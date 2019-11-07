using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class FactionInteraction : IExposable, ILoadReferenceable
    {
        public int id;

        public Faction Faction;
        public List<InteractionOption> Options;
        public int Trust
        {
            get
            {
                return trust;
            }
            set
            {
                trust = Mathf.Clamp(value, -50, 500);

                if(trust <= 0)
                {
                    var alliance = QuestsManager.Communications.FactionManager.PlayerAlliance;
                    if (alliance != null && alliance.Factions.Contains(this))
                    {
                        alliance.RemoveFaction(this, AllianceRemoveReason.LowTrust);
                    }
                }
            }
        }
        private int trust;

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
            Scribe_Values.Look(ref id, "id");

            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Options, "Options", LookMode.Deep);
            Scribe_Values.Look(ref trust, "Trust");
        }

        public string GetUniqueLoadID()
        {
            return "FactionInteraction_" + id;
        }
    }
}
