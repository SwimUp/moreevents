using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.Events.PlaceBattle
{
    public class PlaceBattle : VisitableSite
    {
        public Faction SecondFaction;

        public override bool ShowButton => false;

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            GenerateDeadOrDownedPawns();

            GenerateAlivePawns();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            CaravanArrivalAction_EnterToRaidMap caravanAction = new CaravanArrivalAction_EnterToRaidMap(this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterMap".Translate(Label), caravan, this.Tile, this);
        }

        private void GenerateAlivePawns()
        {
            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(Map, out IntVec3 first, out IntVec3 second);
            CellFinder.TryFindRandomCellNear(first, Map, 4, (IntVec3 x) => x.Standable(Map) && !x.Fogged(Map), out first);
            CellFinder.TryFindRandomCellNear(second, Map, 4, (IntVec3 x) => x.Standable(Map) && !x.Fogged(Map), out second);

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = Rand.Range(500, 1300),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            var pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            SpawnAndGiveLord(pawns, Faction, Map, first);

            pawnGroupMakerParms.faction = SecondFaction;
            pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            SpawnAndGiveLord(pawns, SecondFaction, Map, second);
        }

        private void SpawnAndGiveLord(List<Pawn> pawns, Faction faction, Map map, IntVec3 spot)
        {
            foreach (var firstFactionPawn in pawns)
            {
                if (CellFinder.TryFindRandomCellNear(spot, map, 6, (IntVec3 x) => x.Standable(map) && !x.Fogged(map), out IntVec3 loc))
                {
                    GenSpawn.Spawn(firstFactionPawn, loc, map, Rot4.Random);
                }
            }
            LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(faction, map.Center);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob_DefendBase, map, pawns);
        }
        private void GenerateDeadOrDownedPawns()
        {
            int burialCount = Rand.Range(10, 25);
            for (int i = 0; i < burialCount; i++)
            {
                if (Map.AllCells.Where(vec => !vec.Fogged(Map) && vec.DistanceToEdge(Map) > 5).TryRandomElement(out IntVec3 result))
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(Faction.RandomPawnKind(), Rand.Chance(0.5f) ? Faction : SecondFaction);

                    GenSpawn.Spawn(pawn, result, Map);

                    if (Rand.Chance(0.65f))
                        pawn.Kill(null);
                    else
                        HealthUtility.DamageUntilDowned(pawn);
                }
            }
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref SecondFaction, "SecondFaction");
        }
    }
}
