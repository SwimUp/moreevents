using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events
{
    public class IncidentWorker_UnifiedRaid : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["UnifiedRaid"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryGetTwoFactions(out Faction faction1, out Faction faction2))
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryGetTwoFactions(out Faction faction1, out Faction faction2))
                return false;

            Map map = (Map)parms.target;

            parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);

            IntVec3 existingMapEdgeCell1 = IntVec3.Invalid;
            IntVec3 existingMapEdgeCell2 = IntVec3.Invalid;
            if (!TryFindEntryCell(map, out existingMapEdgeCell1))
            {
                return false;
            }

            if (!TryFindEntryCell(map, out existingMapEdgeCell2))
            {
                return false;
            }

            GenerateAndSpawnPawns(faction1, parms, existingMapEdgeCell1);
            GenerateAndSpawnPawns(faction2, parms, existingMapEdgeCell2);

            SendStandardLetter();

            return true;
        }

        private void GenerateAndSpawnPawns(Faction faction, IncidentParms parms, IntVec3 spot)
        {
            Map map = (Map)parms.target;

            LordJob lordJob = new LordJob_AssaultColony(faction, canKidnap: true, canTimeoutOrFlee: false, true);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map);

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = faction,
                points = Mathf.Clamp(parms.points / 2, 0, parms.points),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            IEnumerable<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
            foreach (var pawn in pawns)
            {
                IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(spot, map);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
                lord.AddPawn(pawn);
            }
        }

        private bool TryGetTwoFactions(out Faction faction1, out Faction faction2)
        {
            List<Faction> enemyFactions = Find.FactionManager.AllFactionsInViewOrder.Where(f => f != Faction.OfPlayer && !f.defeated && f.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile).ToList();

            foreach (var fac1 in enemyFactions)
            {
                foreach(var fac2 in enemyFactions)
                {
                    if (fac1 == fac2)
                        continue;

                    if (fac1.RelationKindWith(fac2) == FactionRelationKind.Ally || fac1.RelationKindWith(fac2) == FactionRelationKind.Neutral)
                    {
                        faction1 = fac1;
                        faction2 = fac2;

                        return true;
                    }
                }
            }

            faction1 = null;
            faction2 = null;

            return false;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => x.Standable(map) && map.reachability.CanReachColony(x), map, CellFinder.EdgeRoadChance_Hostile, out cell);
        }
    }
}
