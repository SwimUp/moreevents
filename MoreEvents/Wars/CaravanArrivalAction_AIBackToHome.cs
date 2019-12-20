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
    public class CaravanArrivalAction_AIBackToHome : CaravanArrivalAction
    {
        public override string Label => "CaravanArrivalAction_AIBackToHome_Label".Translate();

        public override string ReportString => "CaravanArrivalAction_AIBackToHome_Label".Translate();

        private MapParent mapParent;

        public CaravanArrivalAction_AIBackToHome()
        {

        }

        public CaravanArrivalAction_AIBackToHome(MapParent target)
        {
            this.mapParent = target;
        }

        public override void Arrived(Caravan caravan)
        {
            caravan.RemoveAllPawns();

            if (caravan.Spawned)
            {
                Find.WorldObjects.Remove(caravan);
            }
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mapParent, "mapParent");
        }
    }
}
