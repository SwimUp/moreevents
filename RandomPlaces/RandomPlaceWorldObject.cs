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

        public CompRandomPlace Worker;

        public override bool AppendFactionToInspectString => false;

        protected override bool UseGenericEnterMapFloatMenuOption => UseEnterMapFloatMenuOption;
        public bool UseEnterMapFloatMenuOption = true;


        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            if (MapDef != null)
            {
                MapGeneratorHandler.GenerateMap(MapDef, Map, out List<Pawn> pawns, true, true, true, false, true, true, true, Faction);
            }

            Worker?.PostMapGenerate(Map);
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

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref UseEnterMapFloatMenuOption, "UseEnterMapFloatMenuOption");
        }
    }
}
