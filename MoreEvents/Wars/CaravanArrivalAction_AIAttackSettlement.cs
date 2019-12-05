using QuestRim;
using RimOverhaul.AI;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Wars
{
    public class CaravanArrivalAction_AIAttackSettlement : CaravanArrivalAction
    {
        public override string Label => mapParent.LabelCap;

        public override string ReportString => mapParent.LabelCap;

        private MapParent mapParent;

        public float SkillMultiplier => 10;

        public CaravanArrivalAction_AIAttackSettlement()
        {

        }

        public CaravanArrivalAction_AIAttackSettlement(MapParent target)
        {
            this.mapParent = target;
        }

        public override void Arrived(Caravan caravan)
        {
            AttackNow(caravan);
        }

        public virtual void AttackNow(Caravan caravan)
        {
            if(mapParent.HasMap)
            {
                EnterArmy(caravan);

                return;
            }

            float attackPoints = FinalizeAttackPoints(caravan);
            float defendersPoints = WarUtility.ThreatRangeFor(mapParent.Faction.def.techLevel).RandomInRange;


        }

        private float FinalizeAttackPoints(Caravan caravan)
        {
            Faction faction = caravan.Faction;

            float result = (((CaravanAI)caravan).Threat + GetLeaderPoints(faction)) * WarUtility.GetMultiplierFor(faction.def.techLevel);

            return result;
        }

        private float FinalizeDefendersPoints()
        {
            Faction faction = mapParent.Faction;
            Tile tile = Find.WorldGrid[mapParent.Tile];

            float points = ((WarUtility.ThreatRangeFor(faction.def.techLevel).RandomInRange + GetLeaderPoints(faction)) * WarUtility.GetMultiplierFor(faction.def.techLevel))
                * WarUtility.GetMultiplierFor(tile.hilliness);

            return points;
        }

        private float GetLeaderPoints(Faction faction)
        {
            float leaderPoints = 0f;

            Pawn factionLeader = faction.leader;
            if (factionLeader != null)
            {
                foreach (var skill in factionLeader.skills.skills)
                {
                    if (!skill.TotallyDisabled && (skill.def == SkillDefOf.Melee || skill.def == SkillDefOf.Shooting || skill.def == SkillDefOf.Intellectual))
                    {
                        leaderPoints += (skill.Level * SkillMultiplier);
                    }
                }
            }

            return leaderPoints;
        }

        private void EnterArmy(Caravan caravan)
        {
            List<Pawn> pawns = new List<Pawn>(caravan.pawns.InnerListForReading);

            Map map = mapParent.Map;
            IncidentParms parms = new IncidentParms
            {
                target = map,
                faction = pawns.First().Faction
            };

            Find.WorldObjects.Remove(caravan);

            CellFinder.TryFindRandomEdgeCellWith(x => x.Walkable(map) && !x.Fogged(map) && x.Standable(map), map, 0f, out parms.spawnCenter);
            var arrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            arrivalMode.Worker.Arrive(pawns, parms);

            RaidStrategyDef raidStrategy = RaidStrategyDefOf.ImmediateAttack;

            raidStrategy.Worker.MakeLords(parms, pawns);
        }

        public virtual FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            FloatMenuAcceptanceReport floatMenuAcceptanceReport = true;
            if (!(bool)floatMenuAcceptanceReport)
            {
                return floatMenuAcceptanceReport;
            }
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }
            return CanVisit(caravan, mapParent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mapParent, "mapParent");
        }
    }
}
