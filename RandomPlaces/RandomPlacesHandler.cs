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
            Triggers.Clear();

            for(int i = 0; i < allPlaces.Count; i++)
            {
                RandomPlaceDef randomPlaceDef = allPlaces[i];

                int spawnedCount = 0;

                for (int i3 = 0; i3 < randomPlaceDef.MinAtStart; i3++)
                {
                    int tile = TileFinder.RandomStartingTile();

                    if (randomPlaceDef.Worker == null || randomPlaceDef.Worker.CanPlace(tile))
                    {
                        TryGetFaction(randomPlaceDef.FactionType, out Faction faction);
                        Triggers.Add(tile, MakeTrigger(tile, randomPlaceDef, faction));
                        spawnedCount++;

                    }
                }

               if (spawnedCount >= randomPlaceDef.MaxSpawn)
                continue;

                for (int i4 = 0; i4 < randomPlaceDef.MaxSpawn - spawnedCount; i4++)
                {
                    if (Rand.Chance(randomPlaceDef.Commonality))
                    {
                        int tile = TileFinder.RandomStartingTile();
                        if (randomPlaceDef.Worker == null || randomPlaceDef.Worker.CanPlace(tile))
                        {
                            TryGetFaction(randomPlaceDef.FactionType, out Faction faction);
                            Triggers.Add(tile, MakeTrigger(tile, randomPlaceDef, faction));
                        }
                    }
                }
            }

            if(Prefs.DevMode)
            {
                Log.Message($"TOTAL: {Triggers.Count}");
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

        public WorldTrigger MakeTrigger(int tile, RandomPlaceDef def, Faction faction)
        {
            WorldTrigger trigger = new WorldTrigger(tile, def, faction);

            return trigger;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Triggers, "Triggers", LookMode.Value, LookMode.Deep);
        }
    }
}
