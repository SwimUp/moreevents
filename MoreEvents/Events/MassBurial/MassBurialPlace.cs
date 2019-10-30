using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events.MassBurial
{
    public class MassBurialPlace : VisitableSite
    {
        private int raidTimer;
        private bool raidSended = false;

        public override bool ShowButton => false;
        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            GenerateBurial();

            Faction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile, reason: "MassBurialPlace_WhyAffect".Translate());

            raidTimer = (int)(Rand.Range(0.2f, 0.5f) * 60000);
            raidSended = false; 
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref raidSended, "raidSended");
            Scribe_Values.Look(ref raidTimer, "raidTimer");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            CaravanArrivalAction_EnterToRaidMap caravanAction = new CaravanArrivalAction_EnterToRaidMap(this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterMap".Translate(Label), caravan, this.Tile, this);
        }

        public override void Tick()
        {
            base.Tick();

            if(HasMap && !raidSended)
            {
                raidTimer--;

                if(raidTimer <= 0)
                {
                    raidSended = true;

                    SendRaid();
                }
            }
        }

        private void SendRaid()
        {
            int @int = Rand.Int;
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Map);
            raidParms.forced = true;
            raidParms.faction = Faction;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            CellFinder.TryFindRandomEdgeCellWith(x => !x.Fogged(Map) && x.Standable(Map) && x.Walkable(Map), Map, 0f, out raidParms.spawnCenter);
            raidParms.points = Rand.Range(500, 1000);
            raidParms.pawnGroupMakerSeed = @int;
            var incident = new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms);
            Find.Storyteller.TryFire(incident);
        }

        private void GenerateBurial()
        {
            int burialCount = Rand.Range(15, 25);
            for(int i = 0; i < burialCount; i++)
            {
                if (Map.AllCells.Where(vec => !vec.Fogged(Map) && vec.DistanceToEdge(Map) > 5).TryRandomElement(out IntVec3 result))
                {
                    Building_Grave building_Grave = (Building_Grave)ThingMaker.MakeThing(ThingDefOfLocal.Grave);
                    Pawn pawn = PawnGenerator.GeneratePawn(Faction.RandomPawnKind());

                    building_Grave.GetDirectlyHeldThings().TryAdd(pawn);

                    GenSpawn.Spawn(building_Grave, result, Map);

                    pawn.Kill(null);
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

        public override string GetDescription()
        {
            return string.Format(base.GetDescription(), Faction.Name);
        }
    }
}
