using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class CaravanArrivalAction_EnterToQuestMap: CaravanArrivalAction_Enter
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public virtual IntVec3 MapSize { get; set; }

        public override string Label => "EnterMap".Translate(mapParent.Label);

        public override string ReportString => "CaravanEntering".Translate(mapParent.Label);

        public QuestSite QuestSite => questSite;
        public QuestSite questSite;

        public CaravanArrivalAction_EnterToQuestMap()
        {

        }

        public CaravanArrivalAction_EnterToQuestMap(MapParent mapParent)
        {
            this.mapParent = mapParent;
            this.questSite = (QuestSite)mapParent;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref questSite, "questSite");
            Scribe_References.Look(ref mapParent, "MapParent");
        }

        public override void Arrived(Caravan caravan)
        {
            Enter(caravan);
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

        public virtual void DoEnter(Caravan caravan)
        {
            Pawn t = caravan.PawnsListForReading[0];
            bool flag2 = !mapParent.HasMap;
            Verse.Map orGenerateMap = GetOrGenerateMap(mapParent.Tile, MapSize, null);
            if (flag2)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsSite".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, questSite.Label).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {questSite.Label}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {questSite.Label}", "LetterCaravanEnteredMap".Translate(caravan.Label, questSite.Label).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            CaravanEnterMode enterMode = CaravanEnterMode.Edge;
            CaravanEnterMapUtility.Enter(caravan, map, enterMode, CaravanDropInventoryMode.DoNotDrop);
        }

        public virtual Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            return GetOrGenerateMapUtility.GetOrGenerateMap(mapParent.Tile, MapSize, null);
        }
    }
}
