using MapGenerator;
using MoreEvents.MapGeneratorFactionBase;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class FriendlySettlementComp : WorldObjectComp
    {
        public int TicksToAttack = 0;
        public bool Enable;
        public float DaysToAttack => GenDate.TicksToDays(TicksToAttack);

        public FriendlySettlement settlement;

        private List<Pawn> friendlyPawns = new List<Pawn>();
        private List<Pawn> enemyPawns = new List<Pawn>();
        public bool ShowThreat = false;
        public float Points = 0f;
        public Faction OffensiveFaction;

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);

            settlement = (FriendlySettlement)parent;
            Enable = true;
        }

        public void InitPoints()
        {
            Points = Rand.Range(400, 1000) * (int)OffensiveFaction.def.techLevel;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref TicksToAttack, "TicksToAttack");
            Scribe_Values.Look(ref Enable, "Enable");
            Scribe_References.Look(ref OffensiveFaction, "offensiveFaction");
            Scribe_Values.Look(ref Points, "points");
            Scribe_Collections.Look(ref enemyPawns, "enemyPawns", LookMode.Reference);
            Scribe_Collections.Look(ref friendlyPawns, "friendlyPawns", LookMode.Reference);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("TimerDaysToAttack".Translate(DaysToAttack.ToString("f2")));
            if(ShowThreat)
                builder.Append("ThreatLevel".Translate(Points));

            return builder.ToString();
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            BlueprintHandler.CreateBlueprintAt(settlement.Map.Center, settlement.Map, BlueprintDefOfLocal.SiegeCampBase1_1, settlement.Faction, null, out Dictionary<Pawn, LordType> pawns, out float totalThreat, true);
            foreach (var p in pawns.Keys)
                friendlyPawns.Add(p);
        }

        public override void CompTick()
        {
            base.CompTick();

            TicksToAttack--;

            if (Enable)
            {
                if (TicksToAttack <= 0)
                {
                    AttackSetltmenetNow();
                }
            }
        }

        public void AttackSetltmenetNow()
        {
            Enable = false;
            TicksToAttack = 0;
            if (settlement.HasMap)
            {
                PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
                {
                    faction = OffensiveFaction,
                    points = Points,
                    generateFightersOnly = true,
                    groupKind = PawnGroupKindDefOf.Combat,
                    raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                    forceOneIncap = true
                };

                TryFindSpawnSpot(settlement.Map, out IntVec3 spawnSpot);

                enemyPawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
                foreach(var p in enemyPawns)
                {
                    GenSpawn.Spawn(p, spawnSpot, settlement.Map);
                }

                LordJob lordJob = new LordJob_AssaultColony(parent.Faction, canKidnap: true, canTimeoutOrFlee: false);
                if (lordJob != null)
                {
                    LordMaker.MakeNewLord(parent.Faction, lordJob, settlement.Map, enemyPawns);
                }
            }
            else
            {

            }
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }
    }
}
