using Verse;
using System.Collections.Generic;
using RimWorld;
using System.Linq;

namespace MoreEvents.Events
{
    public class IncidentWorker_Disease_Fibrodysplasia : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Disease_Fibrodysplasia"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            if(map != null)
            {
                if (map.mapPawns.FreeColonistsCount <= 2)
                    return false;

                return true;
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;
            List<Pawn> pawns = map.mapPawns.FreeColonists.ToList();
            int num = Utility.GiveHediffToRandomColinists(map, pawns, HediffDefOfLocal.Fibrodysplasia, 1, 1);

            Find.LetterStack.ReceiveLetter(def.label.Translate(), def.letterText.Translate(num, HediffDefOfLocal.Fibrodysplasia.label.Translate()), LetterDefOf.NegativeEvent);

            return true;
        }
    }
}
