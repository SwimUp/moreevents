using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events
{
    public class IncidentWorker_Ambush_TwoEnemyFaction : IncidentWorker_Ambush_EnemyFaction
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = parms.target as Map;

            List<Pair<Faction, Faction>> factions = new List<Pair<Faction, Faction>>();
            foreach (var faction in Find.FactionManager.AllFactionsVisible)
            {
                if (faction == Faction.OfPlayer)
                    continue;

                foreach (var faction2 in Find.FactionManager.AllFactionsVisible)
                {
                    if (faction == Faction.OfPlayer || faction == faction2)
                        continue;

                    if(faction.RelationKindWith(faction2) == FactionRelationKind.Hostile)
                    {
                        factions.Add(new Pair<Faction, Faction>(faction, faction2));
                    }
                }
            }
            if (factions.Count == 0)
                return false;

            if (map != null)
            {
                return TryFindEntryCell(map, out IntVec3 cell);
            }

            return CaravanIncidentUtility.CanFireIncidentWhichWantsToGenerateMapAt(parms.target.Tile);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;
            if (map != null)
            {
                return DoExecute(parms);
            }
            LongEventHandler.QueueLongEvent(delegate
            {
                DoExecute(parms);
            }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);

            return true;
        }

        private bool DoExecute(IncidentParms parms)
        {
            List<Pair<Faction, Faction>> factions = new List<Pair<Faction, Faction>>();
            foreach (var faction in Find.FactionManager.AllFactionsVisible)
            {
                if (faction == Faction.OfPlayer)
                    continue;

                foreach (var faction2 in Find.FactionManager.AllFactionsVisible)
                {
                    if (faction2 == Faction.OfPlayer)
                        continue;

                    if (faction.HostileTo(faction2))
                    {
                        factions.Add(new Pair<Faction, Faction>(faction, faction2));
                    }

                }
            }

            Pair<Faction, Faction> pairFactions = factions.RandomElement();
            Pair<List<Pawn>, List<Pawn>> pawns = new Pair<List<Pawn>, List<Pawn>>(new List<Pawn>(), new List<Pawn>());
            parms.faction = pairFactions.First;
            GeneratePawns(parms).ForEach(p => pawns.First.Add(p));

            if (pawns.First.Count == 0)
                return false;

            parms.faction = pairFactions.Second;
            GeneratePawns(parms).ForEach(p => pawns.Second.Add(p));

            if (pawns.Second.Count == 0)
                return false;

            Map map = parms.target as Map;
            bool flag = false;
            IntVec3 existingMapEdgeCell = IntVec3.Invalid;
            if (map == null)
            {
                map = SetupCaravanAttackMap((Caravan)parms.target, pawns.First, pawns.Second);
                flag = true;
            }
            else
            {
                MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out existingMapEdgeCell, out IntVec3 third);
                SpawnEnemies(existingMapEdgeCell, third, pawns.First, pawns.Second, map);
            }

            LordJob lordJob = new LordJob_AssaultColony(pairFactions.Second, canKidnap: true, canTimeoutOrFlee: false);
            if (lordJob != null)
            {
                LordMaker.MakeNewLord(pairFactions.First, lordJob, map, pawns.First);
            }
            LordJob lordJob2 = new LordJob_AssaultColony(pairFactions.First, canKidnap: true, canTimeoutOrFlee: false);
            if (lordJob != null)
            {
                LordMaker.MakeNewLord(pairFactions.Second, lordJob2, map, pawns.Second);
            }

            Caravan caravan = parms.target as Caravan;
            string letterText = string.Format(def.letterText, (caravan == null) ? "yourCaravan".Translate() : caravan.Name, pairFactions.First.Name, pairFactions.Second.Name);
            Find.LetterStack.ReceiveLetter(def.letterLabel, letterText, def.letterDef);
            if (flag)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
            }

            return true;
        }

        protected override List<Pawn> GeneratePawns(IncidentParms parms)
        {
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            defaultPawnGroupMakerParms.generateFightersOnly = true;
            defaultPawnGroupMakerParms.dontUseSingleUseRocketLaunchers = true;
            return PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
        }

        public Map SetupCaravanAttackMap(Caravan caravan, List<Pawn> faction1, List<Pawn> faction2)
        {
            int num = 140;
            Map map = CaravanIncidentUtility.GetOrGenerateMapForIncident(caravan, new IntVec3(num, 1, num), WorldObjectDefOf.Ambush);
            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out IntVec3 playerStartingSpot, out IntVec3 second);
            CaravanEnterMapUtility.Enter(caravan, map, (Pawn x) => CellFinder.RandomSpawnCellForPawnNear(playerStartingSpot, map), CaravanDropInventoryMode.DoNotDrop, draftColonists: true);

            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out second, out IntVec3 third);
            SpawnEnemies(second, third, faction1, faction2, map);

            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(faction1, "LetterRelatedPawnsGroupGeneric".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
            return map;
        }

        private void SpawnEnemies(IntVec3 spot1, IntVec3 spot2, List<Pawn> faction1, List<Pawn> faction2, Map map)
        {
            for (int i = 0; i < faction1.Count; i++)
            {
                IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(spot1, map);
                GenSpawn.Spawn(faction1[i], loc, map, Rot4.Random);
            }
            for (int i = 0; i < faction2.Count; i++)
            {
                IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(spot2, map);
                GenSpawn.Spawn(faction2[i], loc, map, Rot4.Random);
            }
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => x.Standable(map) && map.reachability.CanReachColony(x), map, CellFinder.EdgeRoadChance_Hostile, out cell);
        }
    }
}
