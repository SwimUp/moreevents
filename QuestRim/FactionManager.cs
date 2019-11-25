using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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

        public List<Alliance> Alliances
        {
            get
            {
                if (alliances == null)
                {
                    alliances = new List<Alliance>();
                }

                return alliances;
            }
        }

        private List<Alliance> alliances;
        public Alliance PlayerAlliance => Alliances.FirstOrDefault(x => x.PlayerOwner);

        public int PlayerAggressiveLevel
        {
            get
            {
                return playerAggresiveLevel;
            }
            set
            {
                playerAggresiveLevel = Mathf.Clamp(value, 0, 100);
            }
        }
        private int playerAggresiveLevel;

        public List<War> Wars
        {
            get
            {
                if (wars == null)
                {
                    wars = new List<War>();
                }

                return wars;
            }
        }

        private List<War> wars;

        public FactionManager()
        {
        }

        public void Tick()
        {
            for (int i = 0; i < Alliances.Count; i++)
            {
                var alliance = Alliances[i];

                try
                {
                    alliance.Tick();
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception ticking alliance {alliance.id} --> {ex}");
                }
            }

            if (Find.TickManager.TicksGame % 5000 == 0)
            {
                foreach(var alliance in Alliances)
                {
                    foreach(FiringIncident item in alliance.MakeIncidentsForInterval())
                    {
                        TryFire(item);
                    }
                }
            }
        }

        public bool TryFire(FiringIncident fi)
        {
            if (fi.def.Worker.CanFireNow(fi.parms) && fi.def.Worker.TryExecute(fi.parms))
            {
                fi.parms.target.StoryState.Notify_IncidentFired(fi);
                return true;
            }
            return false;
        }

        public void AddWar(War war)
        {
            if (Wars.Contains(war))
                return;

            Wars.Add(war);
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
                Log.Warning("[FactionManager] Null interaction, create new");
                Add(faction);
            }

            return interaction;
        }

        public void AddAlliance(Alliance alliance, bool sendMessage = true)
        {
            if (alliance.FactionOwner == Faction.OfPlayer)
                alliance.PlayerOwner = true;

            Alliances.Add(alliance);

            if (sendMessage)
            {
                Find.LetterStack.ReceiveLetter("GlobalFactionManager_NewAllianceTitle".Translate(), "GlobalFactionManager_NewAllianceDesc".Translate(alliance.FactionOwner.Name, alliance.AllianceGoalDef.LabelCap), LetterDefOf.NeutralEvent);
            }
        }

        public void Add(Faction faction)
        {
            if(!Factions.Contains(faction))
            {
                InitNewFaction(faction, StandartOptions(faction));
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
                        factions.Remove(interaction);
                    }
                }
            }
        }

        private void InitNewFaction(Faction faction, List<InteractionOption> options)
        {
            FactionInteraction interaction = new FactionInteraction();
            interaction.Faction = faction;
            interaction.id = QuestsManager.Communications.UniqueIdManager.GetNextFactionInteractionId();

            interaction.Options = options;

            factions.Add(interaction);
        }

        public List<InteractionOption> StandartOptions(Faction forFaction)
        {
            var list = new List<InteractionOption>();

            if (forFaction.def.permanentEnemy)
            {
                list.Add(new CommOption_NonAgressionPact());
            }
            else
            {
                list.Add(new CommOption_SubscribeScout());
            }

            if(!forFaction.def.permanentEnemy)
                list.Add(new CommOption_InviteToAlliance());

            list.Add(new CommOption_MakeWar());

            return list;
        }

        public void RecacheFactions()
        {
            if (factions == null)
            {
                factions = new List<FactionInteraction>();
            }

            foreach(var faction in Find.FactionManager.AllFactionsVisible)
            {
                if (!Factions.Contains(faction))
                {
                    InitNewFaction(faction, StandartOptions(faction));
                }
            }

            for (int i = 0; i < Factions.Count; i++)
            {
                Faction faction = Factions[i].Faction;
                if (!Find.FactionManager.AllFactionsVisible.Contains(faction))
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
            Scribe_Collections.Look(ref alliances, "alliances", LookMode.Deep);
            Scribe_Values.Look(ref playerAggresiveLevel, "PlayerAggressiveLevel");
            Scribe_Collections.Look(ref wars, "wars", LookMode.Deep);
        }
    }
}
