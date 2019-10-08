using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Quests
{
    public class CaravanArrivalAction_EnterToQuestMapWithGenerator : CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(MapGeneratorDef.size.x, 1, MapGeneratorDef.size.z);

        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapGeneratorDef;

        public CaravanArrivalAction_EnterToQuestMapWithGenerator() : base()
        {

        }

        public CaravanArrivalAction_EnterToQuestMapWithGenerator(MapParent mapParent, MapGeneratorBlueprints.MapGenerator.MapGeneratorDef mapGeneratorDef) : base(mapParent)
        {
            MapGeneratorDef = mapGeneratorDef;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref MapGeneratorDef, "MapGeneratorDef");
        }

        public override Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            if (map == null)
            {
                Log.Message("GEN: " + mapSize);
                map = Verse.MapGenerator.GenerateMap(mapSize, MapParent, MapGeneratorDefOfLocal.EmptyMap);
            }
            return map;
        }
    }
}
