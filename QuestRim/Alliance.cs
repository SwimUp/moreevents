using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public enum AllianceRemoveReason : byte
    {
        Kick,
        LowTrust,
        None
    }

    public class Alliance : IExposable, ILoadReferenceable
    {
        public int id;

        public bool PlayerOwner;

        public string Name;

        public Faction FactionOwner;

        public AllianceGoalDef AllianceGoalDef;

        public List<FactionInteraction> Factions = new List<FactionInteraction>();

        public Alliance()
        {

        }

        public Alliance(string name, Faction faction, AllianceGoalDef goal)
        {
            Name = name;
            FactionOwner = faction;
            AllianceGoalDef = goal;
        }

        public void AddFaction(FactionInteraction faction)
        {
            Factions.Add(faction);

            SynchonizeRelation(faction, FactionRelationKind.Hostile);
        }

        public void GiveTrustToAllFactions(int trust)
        {
            foreach(var faction in Factions)
            {
                faction.Trust += trust;
            }
        }

        public void RemoveFaction(FactionInteraction faction, AllianceRemoveReason reason)
        {
            Factions.Remove(faction);

            if (reason == AllianceRemoveReason.Kick)
            {
                faction.Faction.TrySetRelationKind(FactionOwner, FactionRelationKind.Hostile);
                faction.Trust -= 100;

                foreach (var allianceFaction in Factions)
                    allianceFaction.Faction.TrySetRelationKind(faction.Faction, FactionRelationKind.Hostile);
            }
        }

        private void SynchonizeRelation(FactionInteraction faction, FactionRelationKind syncRelation)
        {
            foreach (var worldFaction in Find.FactionManager.AllFactionsVisible)
            {
                if (worldFaction == FactionOwner)
                    continue;

                if (Factions.Contains(faction))
                    continue;

                if (worldFaction.RelationKindWith(FactionOwner) == syncRelation)
                {
                    faction.Faction.TrySetRelationKind(worldFaction, syncRelation);
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");

            Scribe_Values.Look(ref PlayerOwner, "PlayerOwner");
            Scribe_References.Look(ref FactionOwner, "FactionOwner");
            Scribe_Defs.Look(ref AllianceGoalDef, "goal");
            Scribe_Collections.Look(ref Factions, "Factions", LookMode.Reference);
            Scribe_Values.Look(ref Name, "Name");
        }

        public string GetUniqueLoadID()
        {
            return "Alliance_" + id;
        }

        public static Alliance MakeAlliance(string name, Faction faction, AllianceGoalDef goal)
        {
            Alliance alliance = new Alliance(name, faction, goal);
            alliance.id = QuestsManager.Communications.UniqueIdManager.GetNextAllianceID();

            return alliance;
        }
    }
}
