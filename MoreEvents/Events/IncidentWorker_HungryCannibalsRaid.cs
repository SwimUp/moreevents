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

            IEnumerable<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
            LordJob lordJob = new LordJob_AssaultColony(tribalFaction, canKidnap: true, canTimeoutOrFlee: false);
            Lord lord = LordMaker.MakeNewLord(tribalFaction, lordJob, map);
            lord.numPawnsLostViolently = int.MaxValue;

            foreach (var pawn in pawns)
            {
                IntVec3 pos = CellFinder.RandomEdgeCell(map);
                GenSpawn.Spawn(pawn, pos, map);
                pawn.health.AddHediff(HediffDefOfLocal.ThirstHumanMeat);
                lord.AddPawn(pawn);
            }

            SendStandardLetter();

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;

            return true;
        }
    }
}
