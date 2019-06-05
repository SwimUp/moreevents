using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class Dangerous_AlreadyStand
    {
        private int maxGroups = 5;
        private int maxPawnsInGroup = 15;

        public Dangerous_AlreadyStand(float threatPoints, Faction faction, Verse.Map map, int maxGroups, int maxPawnsInGroup)
        {
            this.maxGroups = maxGroups;
            this.maxPawnsInGroup = maxPawnsInGroup;

            SpawnEnemies(threatPoints, faction, map);
        }

        private void SpawnEnemies(float threatPoints, Faction faction, Verse.Map map)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = faction,
                points = threatPoints * 0.4f,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            List<int> groups = new List<int>() { pawns.Count };
            int totalPawns = pawns.Count - 1;
            if (pawns.Count > 5)
            {
                groups.Clear();

                if (pawns.Count < maxPawnsInGroup)
                    maxPawnsInGroup = pawns.Count;

                while (groups.Count < maxGroups)
                {
                    if (totalPawns <= 0)
                        break;

                    int pawnsGroupCount = Rand.Range(1, maxPawnsInGroup);

                    if (pawnsGroupCount > totalPawns)
                        pawnsGroupCount = totalPawns;

                    groups.Add(pawnsGroupCount);
                    totalPawns -= pawnsGroupCount;
                }
            }

            int total = 0;
            for(int i = 0; i < groups.Count; i++)
            {
                MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out IntVec3 playerStartingSpot, out IntVec3 second);

                for(int i2 = 0; i2 < groups[i]; i2++)
                {
                    IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(second, map);
                    GenSpawn.Spawn(pawns[total], loc, map);
                    total++;
                }
            }

            LordJob lordJob = new LordJob_AssaultColony(faction, canKidnap: true, canTimeoutOrFlee: false);
            if (lordJob != null)
            {
                LordMaker.MakeNewLord(faction, lordJob, map, pawns);
            }
        }
    }
}
