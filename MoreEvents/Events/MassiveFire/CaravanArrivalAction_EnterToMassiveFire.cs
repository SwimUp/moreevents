﻿using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.MassiveFire
{
    public class CaravanArrivalAction_EnterToMassiveFire : CaravanArrivalAction_Enter
    {
        private MapParent mapParent;

        public static readonly IntVec3 MapSize = new IntVec3(150, 1, 150);

        public override string Label => "EnterMap".Translate(mapParent.Label);

        public override string ReportString => "CaravanEntering".Translate(mapParent.Label);

        private MassiveFireMapSite site;

        public CaravanArrivalAction_EnterToMassiveFire(MapParent mapParent, MassiveFireMapSite site)
        {
            this.mapParent = mapParent;
            this.site = site;
        }

        public override void Arrived(Caravan caravan)
        {
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
            return CaravanArrivalAction_EnterToMassiveFire.CanVisit(caravan, mapParent);
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
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, site).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {site.Label}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {site.Label}", "LetterCaravanEnteredMap".Translate(caravan.Label, site).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            CaravanEnterMode enterMode = CaravanEnterMode.Edge;
            CaravanEnterMapUtility.Enter(caravan, map, enterMode, CaravanDropInventoryMode.DoNotDrop);
        }
    }
}
