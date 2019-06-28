using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class CaravanArrivalAction_GiveRansom : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public override string Label => "GiveRansom".Translate();

        public override string ReportString => "PaidSuccessDoom".Translate();

        public CaravanArrivalAction_GiveRansom()
        {
        }

        public CaravanArrivalAction_GiveRansom(MapParent mapParent)
        {
            this.mapParent = mapParent;
        }

        public override void Arrived(Caravan caravan)
        {
            var comp = ((DoomsdaySite)mapParent).GetComponent<DoomsdayUltimatumComp>();
            int remaining = 50000 - comp.FactionSilver;
            List<Thing> list = CaravanInventoryUtility.TakeThings(caravan, (Thing thing) =>
            {
                if (thing.def == ThingDefOf.Silver)
                {
                    int num = Mathf.Min(remaining, thing.stackCount);
                    remaining -= num;
                    return num;
                }

                return 0;
            });

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Destroy();
            }

            Find.LetterStack.ReceiveLetter("RansomCompleteTitle".Translate(), "RansomComplete".Translate(), LetterDefOf.PositiveEvent);

            Find.WorldObjects.Remove(mapParent);
        }

        public FloatMenuAcceptanceReport CanGiveRansom(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }

            var comp = ((DoomsdaySite)mapParent).GetComponent<DoomsdayUltimatumComp>();
            int remaining = 50000 - comp.FactionSilver;
            bool hasSilver = CaravanInventoryUtility.HasThings(caravan, ThingDefOf.Silver, remaining);

            return hasSilver;
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
            return CanGiveRansom(caravan, mapParent);
        }
    }
}
