using MoreEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class PlaceWorker_GasPlant : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            Thing thing = map.thingGrid.ThingAt(loc, ThingDefOfLocal.GasWell);
            if (thing == null || thing.Position != loc)
            {
                return "MustPlaceOnGasWell".Translate();
            }
            return true;
        }

        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == ThingDefOfLocal.GasWell;
        }
    }
}
