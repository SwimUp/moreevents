using MapGeneratorBlueprints.MapGenerator;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RandomPlaces
{
    public class RandomPlaceWorldObject : MapParent
    {
        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapDef;

        public override bool AppendFactionToInspectString => false;

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            MapGeneratorHandler.GenerateMap(MapDef, Map, out List<Pawn> pawns, true, true, true, false, true, true, true, Faction);
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }
    }
}
