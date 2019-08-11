using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_MechanoidPortalStart : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MechanoidPortal"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryFindCell(out IntVec3 result, (Map)parms.target))
                return false;

            if(Find.CurrentMap.areaManager.Home.TrueCount <= 0)
                return false;
                
            return true;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            if(TryFindCell(out IntVec3 result, map))
            {
                var faller = SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, ThingDefOfLocal.MechanoidTeleport_Generator, result, map);
                SendStandardLetter(new LookTargets(new TargetInfo(result, map)));
                return true;
            }

            return false;
        }

        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out cell, 10, default(IntVec3), -1, true, true, true, true, false, false);
        }
    }
}
