using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MoreEvents.Events.ShipCrash.ShipCrash_Controller;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public abstract class ShipMapGenerator
    {
        public abstract ShipSiteType SiteType { get; }

        public abstract void RunGenerator();
    }
}
