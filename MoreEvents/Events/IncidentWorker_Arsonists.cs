using MoreEvents.AI;
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
    public class IncidentWorker_Arsonists : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Arsonists"];

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            List<Thing> plants = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
            if (plants.Count < 10)
                return false;

            CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(map) && c.Walkable(map) && c.Standable(map), map, 0f, out IntVec3 pos);
            IntVec3 spawnSpot = pos;
            IntVec3 startSpot = CellFinder.RandomClosewalkCellNear(spawnSpot, map, 10);
            Faction enemyFaction = Find.FactionManager.RandomEnemyFaction();

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = enemyFaction,
                points = Rand.Range(400, 700),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat
            };

            LordJob lordJob = new LordJob_Arson(startSpot, enemyFaction);
            Lord lord = LordMaker.MakeNewLord(enemyFaction, lordJob, map);

            IEnumerable<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
            foreach (var p in pawns)
            {
                GenSpawn.Spawn(p, spawnSpot, map);
                lord.AddPawn(p);
            }

            SendStandardLetter(parms, new LookTargets(new TargetInfo(spawnSpot, map)));

            return true;
        }
    }
}
