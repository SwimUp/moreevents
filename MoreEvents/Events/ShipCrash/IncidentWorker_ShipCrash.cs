using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ShipCrash
{
    public class IncidentWorker_ShipCrash : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (ShipCrash_Controller.ShipCount == ShipCrash_Controller.MaxShips)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<int> tiles = new List<int>();
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                Tile tile = Find.WorldGrid[i];
                if (tile.biome != BiomeDefOf.Ocean && tile.biome != BiomeDefOf.Lake && tile.hilliness != Hilliness.Impassable)
                {
                    tiles.Add(i);
                }
            }

            if (tiles.Count == 0)
                return false;

            int tileID = tiles.RandomElement();

            Faction f = Find.FactionManager.RandomEnemyFaction();
            if (f == null)
                return false;

            ShipCrash_Controller.MakeShipPart(new ShipCargo_Food(), tileID, f);

            return true;
        }
    }
}
