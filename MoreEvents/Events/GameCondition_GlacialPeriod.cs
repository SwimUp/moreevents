using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_GlacialPeriod: GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["GlacialPeriod"];

        private bool changeMap = false;

        private List<Tile> RemainingTiles = new List<Tile>();

        private SimpleCurve curveTilesCountForPlanetCoverage = new SimpleCurve
        {
            new CurvePoint(3, 40),
            new CurvePoint(5, 100),
            new CurvePoint(10, 350)
        };

        public override void Init()
        {
            base.Init();

            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            RemainingTiles = Find.WorldGrid.tiles.Where(x => x.temperature > -100).ToList();

            if (RemainingTiles.Count > 0)
            {
                RemainingTiles.Shuffle();
                changeMap = true;
            }
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (!changeMap)
                return;

            if(Find.TickManager.TicksGame % 80000 == 0)
            {
                DoChangeBiome();
            }
        }

        private void DoChangeBiome()
        {
            if (RemainingTiles.Count <= 0)
            {
                changeMap = false;
                return;
            }

            Log.Message("CHANGE");

            int upBorder = Mathf.Min((int)curveTilesCountForPlanetCoverage.Evaluate(Find.World.PlanetCoverage), RemainingTiles.Count);
            int toChange = Rand.Range(1, upBorder);
            for (int i = RemainingTiles.Count - 1, j = 0; j < toChange; j++, i--)
            {
                Tile tile = RemainingTiles[i];
                tile.temperature -= Rand.Range(10, 17);
                tile.biome = GetBiome(tile);

                if(tile.temperature <= -100)
                    RemainingTiles.Remove(tile);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            RemainingTiles = Find.WorldGrid.tiles.Where(x => x.temperature > -100).ToList();

            if (RemainingTiles.Count > 0)
            {
                RemainingTiles.Shuffle();
                changeMap = true;
            }
        }

        private BiomeDef GetBiome(Tile tile)
        {
            if(tile.temperature < -70)
                return BiomeDefOf.IceSheet;

            return tile.biome;
        }
    }
}
