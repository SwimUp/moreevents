using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ShipCrash
{
    public class CaravanArrivalAction_EnterToShipCrash : CaravanArrivalAction_Enter
    {
        private MapParent mapParent;

        public static readonly IntVec3 MapSize = new IntVec3(200, 1, 200);

        private ShipMapGenerator generator;

        public CaravanArrivalAction_EnterToShipCrash(MapParent mapParent, ShipMapGenerator generator)
        {
            this.mapParent = mapParent;
            this.generator = generator;
        }

        public override void Arrived(Caravan caravan)
        {
        //    ShipSite site = (ShipSite)mapParent;
        //    site.OnMap = true;

            Enter(caravan);
        }

        public static FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
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
            return CaravanArrivalAction_EnterToShipCrash.CanVisit(caravan, mapParent);
        }

        public void Enter(Caravan caravan)
        {
            if (!mapParent.HasMap)
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    DoEnter(caravan);
                }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
            }
            else
            {
                DoEnter(caravan);
            }
        }

        public void DoEnter(Caravan caravan)
        {
            Pawn t = caravan.PawnsListForReading[0];
            bool flag2 = !mapParent.HasMap;
            Verse.Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(mapParent.Tile, MapSize, null);
            if (flag2)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsSite".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, generator.ExpandLabel).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter("LetterLabelCaravanEnteredMap".Translate(generator.ExpandLabel), stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("LetterLabelCaravanEnteredMap".Translate(generator.ExpandLabel), "LetterCaravanEnteredMap".Translate(caravan.Label, generator.ExpandLabel).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            CaravanEnterMode enterMode = CaravanEnterMode.Edge;
            CaravanEnterMapUtility.Enter(caravan, map, enterMode, CaravanDropInventoryMode.DoNotDrop);
        }
    }
}
