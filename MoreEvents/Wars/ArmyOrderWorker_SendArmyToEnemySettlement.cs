using RimOverhaul.AI;
using RimOverhaul.Wars;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using Verse;
using static QuestRim.Wars.FactionArmy;

namespace QuestRim.Wars
{
    public class ArmyOrderWorker_SendArmyToEnemySettlement : ArmyOrderWorker
    {
        protected override bool GiveWorkerTo(FactionArmy army, War war)
        {
            List<FactionInteraction> enemyis = war.GetEnemyList(army.Faction);

            if (enemyis != null)
            {
                if (Find.WorldObjects.Settlements.Where(x => x.Faction == army.Faction.Faction).TryRandomElement(out Settlement yourSettlement))
                {
                    //Settlement target = null;
                    //Observable.Start(() => TryFindEnemySettlement(yourSettlement, enemyis, out target)).ObserveOnMainThread().Subscribe(hasSettlement =>
                    //{
                    //    if (hasSettlement)
                    //    {
                    //        bool showCaravanInfo = (war.DeclaredWarFaction == war.PlayerInteraction && war.AssaultFactions.Contains(army.Faction))
                    //        || (war.DefendingFaction == war.PlayerInteraction && war.DefendingFactions.Contains(army.Faction));

                    //        float points = army.Faction.Faction.def.techLevel.ThreatRangeFor().RandomInRange * (1 - army.DepletionFromWar);

                    //        CaravanAI caravanAI = CaravanAIMaker.MakeCaravan(GeneratePawns(army, points), army.Faction.Faction, yourSettlement.Tile, true, CaravanAIMaker.GetCaravanColor(yourSettlement.Faction, target.Faction), showCaravanInfo, showCaravanInfo, false);
                    //        caravanAI.Threat = points;
                    //        caravanAI.Home = yourSettlement;

                    //        caravanAI.AddQueueAction(new CaravanArrivalAction_AIAttackSettlement(target, war), target.Tile);
                    //    }
                    //});

                    if (TryFindEnemySettlement(yourSettlement, enemyis, out Settlement target))
                    {
                        bool showCaravanInfo = (war.DeclaredWarFaction == war.PlayerInteraction && war.AssaultFactions.Contains(army.Faction))
                        || (war.DefendingFaction == war.PlayerInteraction && war.DefendingFactions.Contains(army.Faction));

                        float points = army.Faction.Faction.def.techLevel.ThreatRangeFor().RandomInRange * (1 - army.DepletionFromWar);

                        CaravanAI caravanAI = CaravanAIMaker.MakeCaravan(GeneratePawns(army, points), army.Faction.Faction, yourSettlement.Tile, true, CaravanAIMaker.GetCaravanColor(yourSettlement.Faction, target.Faction), showCaravanInfo, showCaravanInfo, false);
                        caravanAI.Threat = points;
                        caravanAI.Home = yourSettlement;
                        caravanAI.ShowGizmos = false;

                        caravanAI.AddQueueAction(new CaravanArrivalAction_AIAttackSettlement(target, war), target.Tile);
                    }
                }
            }

            return true;
        }

        private IEnumerable<Pawn> GeneratePawns(FactionArmy army, float newPoints)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = army.Faction.Faction,
                points = newPoints,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            foreach(var pawn in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms))
            {
                if (pawn.holdingOwner != null)
                    pawn.holdingOwner.Remove(pawn);

                yield return pawn;
            }
        }

        private bool TryFindEnemySettlement(Settlement root, List<FactionInteraction> enemies, out Settlement resultTarget)
        {
            resultTarget = null;

            Settlement settlement = null;
            Find.WorldFloodFiller.FloodFill(root.Tile, (int x) => !Find.World.Impassable(x), delegate (int tile, int traversalDistance)
            {
                Settlement tmp = Find.WorldObjects.SettlementAt(tile);
                if (tmp != null && enemies.Contains(tmp.Faction))
                {
                    settlement = tmp;
                    return true;
                }

                return false;
            });

            resultTarget = settlement;

            return resultTarget != null;
        }

    }
}
