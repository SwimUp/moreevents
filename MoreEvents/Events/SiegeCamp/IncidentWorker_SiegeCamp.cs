using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.SiegeCamp
{
    public class IncidentWorker_SiegeCamp : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["SiegeCamp"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (GetEnemyFaction() == null)
                return false;

            if (GetPlace((Map)parms.target) == -1)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            int spawnPoint = GetPlace(map);
            if (spawnPoint >= 0)
            {
                Tile tile = Find.WorldGrid[spawnPoint];
                if (Rand.Chance(0.25f))
                    tile.hilliness = Hilliness.SmallHills;
                else
                    tile.hilliness = Hilliness.Flat;

                SiegeCampSite site = (SiegeCampSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.SiegeCampSite);
                site.Tile = spawnPoint;
                site.SetFaction(GetEnemyFaction());
                site.SetMap(map);
                Find.WorldObjects.Add(site);

                SendStandardLetter(site);

                return true;
            }

            return false;
        }

        private Faction GetEnemyFaction()
        {
            Faction f = Find.FactionManager.RandomEnemyFaction();

            if (f == null)
                return null;

            return f;
        }

        private int GetPlace(Map map)
        {
            int playerTile = map.Tile;
            TileFinder.TryFindPassableTileWithTraversalDistance(playerTile, 3, 6, out int result, (int x) => !Find.WorldObjects.AnyWorldObjectAt(x));

            return result;
        }
    }
}
