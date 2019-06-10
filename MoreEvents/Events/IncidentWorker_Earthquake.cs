using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_Earthquake : IncidentWorker_MakeGameCondition
    {
        private EventSettings settings => Settings.EventsSettings["Earthquake"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            Map map = (Map)parms.target;
            Tile tile = Find.WorldGrid[map.Tile];

            if (tile.hilliness == Hilliness.Flat || tile.hilliness == Hilliness.SmallHills || tile.hilliness == Hilliness.Undefined)
                return false;

            return true;
        }
    }
}
