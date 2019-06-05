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
        private int minParts = 7;
        private int maxParts = 15;

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

            int partsCount = Rand.Range(minParts, maxParts);

            for (int i = 0; i < partsCount; i++)
            {
                int tileID = tiles.RandomElement();

                if (Find.WorldObjects.AnyMapParentAt(tileID))
                    continue;

                Faction f = Find.FactionManager.RandomEnemyFaction();
                if (f == null)
                    return false;

                ShipMapGenerator generator = GetGenerator();
                ShipCrash_Controller.MakeShipPart(generator, tileID, f);
            }

            Find.LetterStack.ReceiveLetter(def.label.Translate(), def.letterText.Translate(), LetterDefOf.NegativeEvent);

            ShipCrash_Controller.ShipCount++;

            return true;
        }

        private ShipMapGenerator GetGenerator()
        {
            while(true)
            {
                if(Rand.Chance(0.36f))
                {
                    return new ShipCargo_Food();
                }
                if (Rand.Chance(0.23f))
                {
                    return new ShipCargo_Mining();
                }
                if(Rand.Chance(0.14f))
                {
                    return new ShipCargo_Complex();
                }
            }
        }
    }
}
