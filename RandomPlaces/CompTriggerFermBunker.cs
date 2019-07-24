using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapGeneratorBlueprints.MapGenerator;
using RimWorld;
using Verse;

namespace RandomPlaces
{
    public class CompTriggerFermBunker : CompTriggerGenNewMap
    {
        public override int CollectThingsRadius => 5;

        public override bool UnFogRooms => false;
        public override bool BreakdownBuildings => true;
        public override bool ClearMap => false;
        public override bool GeneratePlants => false;
        public override bool OneUse => true;

        public override MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapGenDefGetter => DefDatabase<MapGeneratorBlueprints.MapGenerator.MapGeneratorDef>.AllDefsListForReading.Where(x => x.targetTags != null && x.targetTags.Contains("LukeBunker")).RandomElementByWeight(x => x.Commonality);

        public override void PostMapGenerate(Map map, RandomPlaceWorldObject worldObject)
        {
            foreach(var cell in map.AllCells)
            {
                GenSpawn.Spawn(ThingDefOf.Granite, cell, map);
            }
        }

        public override void PostMapDefGenerate(Map map, RandomPlaceWorldObject worldObject)
        {
            FloodFillerFog.DebugFloodUnfog(map.mapPawns.FreeColonists.RandomElement().Position, map);

            CameraJumper.TryJump(map.mapPawns.FreeColonists.RandomElement().Position, map);

            worldObject.UseEnterMapFloatMenuOption = false;

            Find.LetterStack.ReceiveLetter("EnteredToBunkerTitle".Translate(), "EnteredToBunkerDesc".Translate(), LetterDefOf.NeutralEvent);
        }
    }
}
