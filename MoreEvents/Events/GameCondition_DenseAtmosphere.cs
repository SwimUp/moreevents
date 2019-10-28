using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_DenseAtmosphere : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["DenseAtmosphere"];

        private bool changeMap = false;

        private const int LerpTicks = 12000;

        private const float MaxTempOffset = 145f;

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

            if (int.Parse(settings.Parameters["DoMapChange"].Value) == 1)
            {
                RemainingTiles = Find.WorldGrid.tiles.Where(x => x.biome != BiomeDefOf.Desert).ToList();

                if (RemainingTiles.Count > 0)
                {
                    RemainingTiles.Shuffle();
                    changeMap = true;
                }
            }
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (!changeMap)
                return;

            if(Find.TickManager.TicksGame % 120000 == 0)
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

            int upBorder = Mathf.Min((int)curveTilesCountForPlanetCoverage.Evaluate(Find.World.PlanetCoverage), RemainingTiles.Count);
            int toChange = Rand.Range(1, upBorder);
            for (int i = RemainingTiles.Count - 1, j = 0; j < toChange; j++, i--)
            {
                Tile tile = RemainingTiles[i];
                tile.biome = BiomeDefOf.Desert;

                RemainingTiles.RemoveAt(i);
            }
        }

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 12000f, MaxTempOffset);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (int.Parse(settings.Parameters["DoMapChange"].Value) == 1)
            {
                RemainingTiles = Find.WorldGrid.tiles.Where(x => x.biome != BiomeDefOf.Desert).ToList();

                if (RemainingTiles.Count > 0)
                {
                    RemainingTiles.Shuffle();
                    changeMap = true;
                }
            }
        }
    }
}
