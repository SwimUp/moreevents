using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map
{
    public class ShipCrashWorker : WorldObjectComp
    {
        public ShipSiteType SiteType = ShipSiteType.Living;
    }
}
