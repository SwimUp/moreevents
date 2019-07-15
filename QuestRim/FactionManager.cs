using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class FactionManager : IExposable
    {
        public List<FactionInteraction> Factions
        {
            get
            {
                if (factions == null)
                {
                    factions = new List<FactionInteraction>();
                    RecacheFactions();
                }

                return factions;
            }
        }
        private List<FactionInteraction> factions;

        public FactionManager()
        {
        }

        public FactionInteraction GetInteraction(Faction faction)
        {
            FactionInteraction interaction = null;
            foreach(var f in Factions)
            {
                if (f.Faction == faction)
                {
                    interaction = f;
                }
            }

            if(interaction == null)
            {
                Log.Warning("[FactionManager]Null interaction, create new");
                Add(faction);
            }

            return interaction;
        }

        public void Add(Faction faction)
        {
            if(!Factions.Contains(faction))
            {
                InitNewFaction(faction, StandartOptions());
            }
        }

        public void Add(Faction faction, List<InteractionOption> options)
        {
            if (!Factions.Contains(faction))
            {
                InitNewFaction(faction, options);
            }
        }

        public void Remove(Faction faction)
        {
            if (Factions.Contains(faction))
            {
                for(int i = 0; i < Factions.Count; i++)
                {
                    FactionInteraction interaction = Factions[i];
                    if(interaction.Faction == faction)
                    {
                        Factions.Remove(interaction);
                    }
                }
            }
        }

        private void InitNewFaction(Faction faction, List<InteractionOption> options)
        {
            FactionInteraction interaction = new FactionInteraction();
            interaction.Faction = faction;
            interaction.Options = options;

            Factions.Add(interaction);
        }

        public List<InteractionOption> StandartOptions()
        {
            var list = new List<InteractionOption>();
            list.Add(new CommOption_SubscribeScout());

            return list;
        }

        public void RecacheFactions()
        {
            if (factions == null)
                return;

            foreach(var faction in Find.FactionManager.AllFactions)
            {
                if (faction == Faction.OfPlayer)
                    continue;

                if(!Factions.Contains(faction))
                {
                    if (faction == Faction.OfAncients || faction == Faction.OfAncientsHostile || faction == Faction.OfInsects || faction == Faction.OfMechanoids || faction == Faction.OfPlayer)
                        continue;

                    Add(faction);
                }
            }

            for(int i = 0; i < Factions.Count; i++)
            {
                Faction faction = Factions[i].Faction;
                if(!Find.FactionManager.AllFactions.Contains(faction))
                {
                    Remove(faction);
                }
            }

        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                RecacheFactions();
            }

            Scribe_Collections.Look(ref factions, "Factions", LookMode.Deep);
        }
    }
}
