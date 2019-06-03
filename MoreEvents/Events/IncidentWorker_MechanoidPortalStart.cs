using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_MechanoidPortalStart : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!TryFindCell(out IntVec3 result, (Map)parms.target))
                return false;

            if(Find.CurrentMap.areaManager.Home.TrueCount <= 0)
                return false;
                
            return true;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;
            if(TryFindCell(out IntVec3 result, map))
            {
                SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, ThingDefOfLocal.MechanoidTeleport_Generator, result, map);
                Find.LetterStack.ReceiveLetter(def.label.Translate(), def.letterText.Translate(), LetterDefOf.NegativeEvent);
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
