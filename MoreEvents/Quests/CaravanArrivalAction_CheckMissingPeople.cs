using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Quests
{
    public class CaravanArrivalAction_CheckMissingPeople : CaravanArrivalAction
    {
        public IntVec3 MapSize => new IntVec3(200, 1, 200);

        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Quest_MissingPeople Quest => quest;
        private Quest_MissingPeople quest;

        public override string Label => "Quest_MissingPeople_CardLabel".Translate();

        public override string ReportString => "Quest_MissingPeople_CardLabel".Translate();

        public CaravanArrivalAction_CheckMissingPeople()
        {
        }

        public CaravanArrivalAction_CheckMissingPeople(MapParent mapParent, Quest_MissingPeople quest)
        {
            this.mapParent = mapParent;
            this.quest = quest;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref mapParent, "Parent");
            Scribe_References.Look(ref quest, "Quest");
        }

        public void Enter(Caravan caravan)
        {
            if(mapParent.HasMap)
                Current.Game.DeinitAndRemoveMap(mapParent.Map);

            LongEventHandler.QueueLongEvent(delegate
            {
                DoEnter(caravan);
            }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);

        }

        public void DoEnter(Caravan caravan)
        {
            Pawn t = caravan.PawnsListForReading[0];
            bool flag2 = !mapParent.HasMap;
            Verse.Map map = GetOrGenerateMap(mapParent.Tile, MapSize, null);
            Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
            Find.LetterStack.ReceiveLetter("Quest_MissingPeople_EnterToMapTitle".Translate(), "Quest_MissingPeople_EnterToMap".Translate(), LetterDefOf.NeutralEvent);

            CaravanEnterMode enterMode = CaravanEnterMode.Edge;
            CaravanEnterMapUtility.Enter(caravan, map, enterMode, CaravanDropInventoryMode.DoNotDrop);

            Quest.UnlimitedTime = true;
        }

        public Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            return GetOrGenerateMapUtility.GetOrGenerateMap(mapParent.Tile, MapSize, null);
        }

        public override void Arrived(Caravan caravan)
        {
            if(Rand.Chance(Quest.FindChance))
            {
                Enter(caravan);
            }
            else
            {
                TryGetNewTile(mapParent.Tile, out int newTile);
                mapParent.Tile = newTile;

                Find.LetterStack.ReceiveLetter("Quest_MissingPeople_CheckFailTitle".Translate(), "Quest_MissingPeople_CheckFail".Translate(), LetterDefOf.NegativeEvent);
            }
        }

        private bool TryGetNewTile(int root, out int newTile)
        {
            return TileFinder.TryFindPassableTileWithTraversalDistance(root, 1, 2, out newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i));
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }

            return true;
        }
    }
}
