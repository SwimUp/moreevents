using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events
{
    public class IncidentWorker_HungryCannibalsRaid : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["HungryCannibalRaid"];
        private Faction tribalFaction => Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.TribeRough);

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (tribalFaction == null)
                return false;

            if (tribalFaction.PlayerRelationKind != FactionRelationKind.Hostile)
                return false;

            Map map = (Map)parms.target;

            parms.points = StorytellerUtility.DefaultThreatPointsNow(map) * 1.2f;

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = tribalFaction,
                points = parms.points,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            if (pawns.Count == 0)
                return false;

            LordJob lordJob = new LordJob_AssaultColony(tribalFaction, canKidnap: true, canTimeoutOrFlee: false);
            Lord lord = LordMaker.MakeNewLord(tribalFaction, lordJob, map);
            lord.numPawnsLostViolently = int.MaxValue;

            foreach (var pawn in pawns)
            {
                if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(map) && c.Walkable(map) && c.Standable(map), map, 0f, out IntVec3 pos))
                {
                    GenSpawn.Spawn(pawn, pos, map);
                    pawn.health.AddHediff(HediffDefOfLocal.ThirstHumanMeat);
                    lord.AddPawn(pawn);
                }
            }

            SendStandardLetter();

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;

            return true;
        }
    }
}
