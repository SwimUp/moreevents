using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace RimOverhaul.AI
{
    public class CaravanArrivalAction_AttackCaravanAI : CaravanArrivalAction
    {
        public IntVec3 MapSize => new IntVec3(200, 1, 200);

        public override string Label => "CaravanArrivalAction_AttackCaravanAI_Label".Translate();

        public override string ReportString => "CaravanArrivalAction_AttackCaravanAI_ReportString".Translate(attackedCaravan.Name);

        public CaravanAI AttackedCaravan => attackedCaravan;
        private CaravanAI attackedCaravan;

        public CaravanArrivalAction_AttackCaravanAI()
        {
        }

        public CaravanArrivalAction_AttackCaravanAI(CaravanAI caravan)
        {
            attackedCaravan = caravan;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref attackedCaravan, "attackedCaravan");
        }

        public void Enter(Caravan caravan)
        {
            DoEnter(caravan);
        }

        public void DoEnter(Caravan caravan)
        {
            CaravanBattlePlacePlace caravanBattlePlacePlace = (CaravanBattlePlacePlace)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.CaravanBattlePlace);
            caravanBattlePlacePlace.Tile = attackedCaravan.Tile;
            caravanBattlePlacePlace.SetFaction(attackedCaravan.Faction);

            Find.WorldObjects.Add(caravanBattlePlacePlace);

            caravanBattlePlacePlace.SetPlace(attackedCaravan, caravan, MapSize);
        }

        public override void Arrived(Caravan caravan)
        {
            if (attackedCaravan == null)
                return;

            LongEventHandler.QueueLongEvent(delegate
            {
                Enter(caravan);
            }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (attackedCaravan == null)
                return false;

            return true;
        }

        public virtual FloatMenuAcceptanceReport CanAttack(Caravan caravan, CaravanAI caravanAI)
        {
            if (caravanAI == null || !caravanAI.Spawned)
            {
                return false;
            }
            return true;
        }
    }
}
