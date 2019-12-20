using QuestRim;
using QuestRim.Wars;
using RimOverhaul.AI;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Wars
{
    public class CaravanArrivalAction_AIAttackSettlement : CaravanArrivalAction
    {
        public override string Label => mapParent.LabelCap;

        public override string ReportString => mapParent.LabelCap;

        public float DepletionMultiplier => 0.00008f;

        private MapParent mapParent;

        public float SkillMultiplier => 10;

        public float AutoWin => 15f;

        public float MultiplierPerDiff => 3;

        public War War;

        public CaravanArrivalAction_AIAttackSettlement()
        {

        }

        public CaravanArrivalAction_AIAttackSettlement(MapParent target, War war)
        {
            this.mapParent = target;
            War = war;
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
            float defendersPoints = FinalizeDefendersPoints();

            float difference = (Mathf.Abs((attackPoints - defendersPoints) / ((attackPoints + defendersPoints) / 2)) * 100) - (caravan.Faction.leader?.skills.GetSkill(SkillDefOf.Intellectual).Level > 15 ? 3 : 0);
            bool attackerWin = false;

            if (difference < AutoWin)
            {
                float winChance = Mathf.Clamp((difference * MultiplierPerDiff) + 50, 0, 100);
                if (Rand.Chance(winChance))
                {
                    attackerWin = attackPoints > defendersPoints;
                }
                else
                {
                    attackerWin = attackPoints < defendersPoints;
                }
            }
            else
            {
                attackerWin = attackPoints > defendersPoints;
            }

            if (attackerWin)
                DoWinAttackers(caravan, defendersPoints);
            else
                DoWinDefenders(caravan, attackPoints);
        }

        public void DoWinAttackers(Caravan caravan, float defendersPoints)
        {
            Find.WorldObjects.Remove(mapParent);

            CaravanAI caravanAI = caravan as CaravanAI;

            float damagePer100Power = defendersPoints / 100;
            AI.CaravanUtility.KillRandomPawns(caravan, damagePer100Power);

            AffectDepletionFromWar(caravan.Faction, (defendersPoints * DepletionMultiplier) * 0.6f, mapParent.Faction, defendersPoints * DepletionMultiplier);

            AI.CaravanUtility.CheckIfCaravanOverWeight(caravan);

            GoBackToHome(caravan);
        }

        public void DoWinDefenders(Caravan caravan, float attackersPoints)
        {
            Find.WorldObjects.Remove(caravan);

            AffectDepletionFromWar(caravan.Faction, attackersPoints * DepletionMultiplier, mapParent.Faction, (attackersPoints * DepletionMultiplier) * 0.6f);
        }

        private void AffectDepletionFromWar(Faction assault, float attackerPoints, Faction defender, float defendersPoints)
        {
            FactionArmy attackerArmy = War.Armys.FirstOrDefault(x => x.Faction.Faction == assault);
            if (attackerArmy != null)
            {
                attackerArmy.DepletionFromWar += attackerPoints;
            }
            FactionArmy defenderArmy = War.Armys.FirstOrDefault(x => x.Faction.Faction == defender);
            if (defenderArmy != null)
            {
                defenderArmy.DepletionFromWar += defendersPoints;
            }
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

            if(map.IsPlayerHome)
            {
                Find.LetterStack.ReceiveLetter("CaravanArrivalAction_AIAttackSettlement_EnemyArmyTitle".Translate(), "CaravanArrivalAction_AIAttackSettlement_EnemyArmyDesc".Translate(caravan.Faction.Name), LetterDefOf.ThreatBig, pawns);
            }
        }

        public virtual FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                GoBackToHome(caravan);
                return false;
            }

            return true;
        }

        public void GoBackToHome(Caravan caravan)
        {
            CaravanAI caravanAI = caravan as CaravanAI;

            if (caravanAI != null && caravanAI.Home != null)
            {
                caravanAI.AddQueueAction(new CaravanArrivalAction_AIBackToHome(caravanAI.Home), caravanAI.Home.Tile);
            }
            else
            {
                Find.WorldObjects.Remove(caravan);
            }
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
            Scribe_References.Look(ref War, "War");
        }
    }
}
