using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreEvents.Events.ShipCrash.Map
{
    public class ShipWorldObjectCompProp_Main : WorldObjectCompProperties
    {
        public ShipWorldObjectCompProp_Main()
        {
            compClass = typeof(ShipCrashWorker);
        }
    }
}
