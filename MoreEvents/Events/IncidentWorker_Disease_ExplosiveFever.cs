using Verse;
using System.Collections.Generic;
using RimWorld;
using System.Linq;

namespace MoreEvents.Events
{
    public class IncidentWorker_Disease_ExplosiveFever : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["ExplosiveFever"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            if(map != null)
            {
                if (map.mapPawns.FreeColonistsCount <= 3)
                    return false;

                return true;
            }

            if (!map.mapPawns.AllPawns.Any(x => x.kindDef == PawnKindDefOfLocal.Boomalope))
                return false;

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            List<Pawn> pawns = map.mapPawns.FreeColonists.ToList();
            int num = Utility.GiveHediffToRandomColinists(map, pawns, HediffDefOfLocal.BlastingBlisters, 1, 1);

            SendStandardLetter();

            return true;
        }
    }
}
