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
            if(reason == AllianceRemoveReason.Kick)
                faction.Faction.TrySetRelationKind(FactionOwner, FactionRelationKind.Hostile);

            Factions.Remove(faction);
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
