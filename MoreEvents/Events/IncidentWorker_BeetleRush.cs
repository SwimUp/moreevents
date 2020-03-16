using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_BeetleRush : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["BeetleRush"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryFindCell(out IntVec3 result, (Map)parms.target))
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            if (TryFindCell(out IntVec3 result, map))
            {
                Thing t = GenSpawn.Spawn(ThingDefOfLocal.HiveCrack, result, map);
                SendStandardLetter(parms, t);
                return true;
            }

            return false;
        }

        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            return CellFinderLoose.TryFindRandomNotEdgeCellWith(15, c => CanSpawnAt(c, map), map, out cell);
        }
        private bool CanSpawnAt(IntVec3 c, Map map)
        {
            if (!c.Standable(map) || c.Fogged(map))
            {
                return false;
            }

            return true;
        }
    }
}
