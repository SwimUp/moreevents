using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.MassiveFire
{
    public class IncidentWorker_MassiveFire : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MassiveFire"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            if (!CanSpawn(map))
                return false;

            return true;
        }

        private bool CanSpawn(Map map)
        {
            int tileID = map.Tile;

            List<int> neight = new List<int>();
            Find.WorldGrid.GetTileNeighbors(tileID, neight);

            if (neight.Count == 0)
                return false;

            foreach(var t in neight)
            {
                Tile tile = Find.WorldGrid[t];

                if (def.allowedBiomes.Contains(tile.biome) && tile.hilliness != Hilliness.Impassable && !Find.WorldObjects.AnyMapParentAt(t))
                    return true;
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            int tileID = map.Tile;

            List<int> neight = new List<int>();
            Find.WorldGrid.GetTileNeighbors(tileID, neight);

            List<int> candidates = new List<int>();
            foreach (var t in neight)
            {
                Tile tile = Find.WorldGrid[t];
                if (def.allowedBiomes.Contains(tile.biome) && tile.hilliness != Hilliness.Impassable && !Find.WorldObjects.AnyMapParentAt(t))
                {
                    candidates.Add(t);
                }
            }

            int spawnTile = candidates.RandomElement();
            candidates.Remove(spawnTile);
            MassiveFireMapSite site = (MassiveFireMapSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.MassiveFireSite);
            site.Candidates = candidates;
            site.Tile = spawnTile;
            site.RootTile = map.Tile;
            site.RootMap = map;
            Find.WorldObjects.Add(site);

            SendStandardLetter(site);

            return true;
        }
    }
}
