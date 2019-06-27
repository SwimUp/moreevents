using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class IncidentWorker_DoomsdayUltimatum : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["DoomsdayUltimatum"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (GetEnemyFaction() == null)
                return false;

            if (GetPlace() == -1)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            int spawnPoint = GetPlace();
            DoomsdaySite site = (DoomsdaySite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.DoomsdayUltimatumCamp);
            site.Tile = spawnPoint;
            site.SetFaction(GetEnemyFaction());
            Find.WorldObjects.Add(site);

            SendStandardLetter(site);

            DoomsdaySite.ActiveSite = site;

            return true;
        }

        private Faction GetEnemyFaction()
        {
            Faction f = Find.FactionManager.RandomEnemyFaction();

            if (f == null)
                return null;

            return f;
        }

        private int GetPlace()
        {
            int result = -1;
            if (TileFinder.TryFindNewSiteTile(out result, 14, 24))
            {
                return result;
            }
            return -1;
        }
    }
}
