using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class IncidentWorker_ClimateBomb : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MechanoidPortal"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<int> tiles = new List<int>();
            for(int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                Tile tile = Find.WorldGrid[i];

                if(tile.hilliness != Hilliness.Impassable && tile.biome != BiomeDefOf.Ocean &&
                    tile.biome != BiomeDefOf.Lake && !Find.WorldObjects.AnyMapParentAt(i))
                {
                    tiles.Add(i);
                }
            }

            if (tiles.Count == 0)
                return false;

            int spawnPoint = tiles.RandomElement();

            ClimateBombSite site = (ClimateBombSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.ClimateBombSite);
            site.Tile = spawnPoint;
            site.GetComponent<ClimateBombComp>().SetTimer();
            Find.WorldObjects.Add(site);

            SendStandardLetter(site);

            return true;
        }
    }
}
