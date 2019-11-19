using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch("Notify_RelationKindChanged")]
    public class Faction__Notify_RelationKindChanged
    {
        public static void Postfix(Faction __instance, Faction other)
        {
            if (__instance == Faction.OfPlayer)
            {
                Alliance alliance = QuestsManager.Communications.FactionManager.PlayerAlliance;

                if (alliance != null)
                {
                    alliance.SynchonizeOwnerRelations(__instance.RelationKindWith(other), other);

                    if(other.RelationKindWith(__instance) == FactionRelationKind.Hostile && alliance.Factions.Contains(other))
                    {
                        FactionInteraction factionInteraction = alliance.Factions.FirstOrDefault(x => x.Faction == other);
                        if (factionInteraction != null)
                            alliance.RemoveFaction(factionInteraction, AllianceRemoveReason.LowTrust);
                    }
                }
            }
        }
    }
}
