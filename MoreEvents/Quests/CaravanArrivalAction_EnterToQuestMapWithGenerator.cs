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

        public bool UseMapSpawnPos;

        public CaravanArrivalAction_EnterToQuestMapWithGenerator() : base()
        {

        }

        public CaravanArrivalAction_EnterToQuestMapWithGenerator(MapParent mapParent, MapGeneratorBlueprints.MapGenerator.MapGeneratorDef mapGeneratorDef, bool useMapSpawnPos = false) : base(mapParent)
        {
            MapGeneratorDef = mapGeneratorDef;
            UseMapSpawnPos = useMapSpawnPos;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref MapGeneratorDef, "MapGeneratorDef");
            Scribe_Values.Look(ref UseMapSpawnPos, "UseMapSpawnPos");
        }

        public override void CaravanEnter(Caravan caravan, Map map)
        {
            if (UseMapSpawnPos)
            {
                CaravanEnterMapUtility.Enter(caravan, map, x => MapGeneratorDef.PawnsSpawnPos, CaravanDropInventoryMode.DoNotDrop);
            }
            else
            {
                CaravanEnterMode enterMode = CaravanEnterMode.Edge;
                CaravanEnterMapUtility.Enter(caravan, map, enterMode, CaravanDropInventoryMode.DoNotDrop);
            }
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
