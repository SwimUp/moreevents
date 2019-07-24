using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public sealed class RandomPlacesHandler : IExposable
    {
        public Dictionary<int, WorldTrigger> Triggers = new Dictionary<int, WorldTrigger>();

        public void InitPlaces()
        {
            List<RandomPlaceDef> allPlaces = DefDatabase<RandomPlaceDef>.AllDefsListForReading;

            for(int i = 0; i < allPlaces.Count; i++)
            {
                RandomPlaceDef randomPlaceDef = allPlaces[i];

                if (Rand.Chance(randomPlaceDef.Commonality))
                {
                    for(int i2 = 0; i2 < randomPlaceDef.Maps.Count; i2++)
                    {
                        int spawnedCount = 0;

                        MapData mapData = randomPlaceDef.Maps[i2];
                        for (int i3 = 0; i3 < mapData.MinAtStart; i3++)
                        {
                            int tile = TileFinder.RandomStartingTile();
                            TryGetFaction(mapData.FactionType, out Faction faction);
                            MakeTrigger(tile, mapData.Map, faction, mapData.Worker);
                            spawnedCount++;
                            Find.LetterStack.ReceiveLetter($"{mapData.Map.defName} - {tile} - {mapData.Worker}", "K", LetterDefOf.Death, new LookTargets(tile));
                        }

                        if (spawnedCount >= mapData.MaxSpawn)
                            continue;

                        for (int i4 = 0; i4 < mapData.MaxSpawn - spawnedCount; i4++)
                        {
                            if (Rand.Chance(mapData.Commonality))
                            {
                                int tile = TileFinder.RandomStartingTile();
                                TryGetFaction(mapData.FactionType, out Faction faction);
                                MakeTrigger(tile, mapData.Map, faction, mapData.Worker);
                                Find.LetterStack.ReceiveLetter($"{mapData.Map.defName} - {tile} - {mapData.Worker}", "G", LetterDefOf.Death, new LookTargets(tile));
                            }
                        }
                    }
                }
            }
        }

        private bool TryGetFaction(FactionRelationKind kind, out Faction faction)
        {
            if ((from f in Find.FactionManager.AllFactionsVisible where f != Faction.OfPlayer && f.PlayerRelationKind == kind select f).TryRandomElement(out faction))
            {
                return true;
            }
            return false;
        }

        public WorldTrigger MakeTrigger(int tile, MapGeneratorBlueprints.MapGenerator.MapGeneratorDef mapGeneratorDef, Faction faction, Type worker)
        {
            WorldTrigger trigger = new WorldTrigger();
            trigger.Tile = tile;
            trigger.Map = mapGeneratorDef;
            trigger.Faction = faction;

            if (worker != null)
            {
                trigger.Worker = (CompRandomPlace)Activator.CreateInstance(worker);
            }

            Triggers.Add(tile, trigger);

            return trigger;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Triggers, "Triggers", LookMode.Value, LookMode.Deep);
        }
    }
}
