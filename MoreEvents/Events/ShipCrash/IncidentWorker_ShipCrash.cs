using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            

            return true;
        }
    }
}
