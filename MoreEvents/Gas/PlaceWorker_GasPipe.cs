using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class PlaceWorker_GasPipe : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            List<Thing> thingList = loc.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                GasManager manager = map.GetComponent<GasManager>();
                ThingDef val = checkingDef as ThingDef;
                CompProperties_GasPipe pipe = val.GetCompProperties<CompProperties_GasPipe>();
                if (manager.PipeAt(loc, pipe.pipeType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
