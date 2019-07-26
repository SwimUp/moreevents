using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimOverhaul.Events
{
    public class IncidentWorker_ConcantrationCamp : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!TileFinder.TryFindNewSiteTile(out int tile))
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TileFinder.TryFindNewSiteTile(out int tile))
                return false;



            return true;
        }
    }
}
