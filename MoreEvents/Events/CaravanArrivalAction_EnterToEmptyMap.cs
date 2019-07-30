using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events
{
    public class CaravanArrivalAction_EnterToEmptyMap : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => mapSize;
        private IntVec3 mapSize;

        public CaravanArrivalAction_EnterToEmptyMap()
        {

        }

        public CaravanArrivalAction_EnterToEmptyMap(MapParent mapParent, IntVec3 mapSize) : base(mapParent)
        {
            this.mapSize = mapSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref mapSize, "mapSize");
        }

        public override Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            if (map == null)
            {
                map = Verse.MapGenerator.GenerateMap(mapSize, MapParent, MapGeneratorDefOfLocal.EmptyMap);
            }
            return map;
        }
    }
}
