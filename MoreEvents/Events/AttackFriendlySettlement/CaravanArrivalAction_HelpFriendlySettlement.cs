using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class CaravanArrivalAction_HelpFriendlySettlement : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(230, 1, 230);

        public CaravanArrivalAction_HelpFriendlySettlement(MapParent mapParent) : base(mapParent)
        {
        }

        public override FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
        }

        public override Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            if (map == null)
            {
                MapParent mapParent = MapParentAt(tile).Where(p => p is FriendlySettlement).FirstOrDefault();
                if (mapParent == null)
                {
                    if (suggestedMapParentDef == null)
                    {
                        Log.Error("Tried to get or generate map at " + tile + ", but there isn't any MapParent world object here and map parent def argument is null.");
                        return null;
                    }
                    mapParent = (MapParent)WorldObjectMaker.MakeWorldObject(suggestedMapParentDef);
                    mapParent.Tile = tile;
                    Find.WorldObjects.Add(mapParent);
                }
                map = Verse.MapGenerator.GenerateMap(mapSize, mapParent, mapParent.MapGeneratorDef, mapParent.ExtraGenStepDefs);
            }
            return map;
        }

        private List<MapParent> MapParentAt(int tile)
        {
            List<MapParent> parentsOnTile = new List<MapParent>();
            for (int i = 0; i < Find.WorldObjects.MapParents.Count; i++)
            {
                if (Find.WorldObjects.MapParents[i].Tile == tile)
                {
                    parentsOnTile.Add(Find.WorldObjects.MapParents[i]);
                }
            }

            return parentsOnTile;
        }
    }
}
