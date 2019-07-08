using Verse;
using System.Collections.Generic;
using RimWorld;
using System.Linq;

namespace MoreEvents.Events
{
    public class IncidentWorker_Disease_NeurofibromatousWorms : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Disease_NeurofibromatousWorms"];

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

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = parms.target as Map;
            List<Pawn> pawns = map.mapPawns.FreeColonists.ToList();
            int num = Rand.Range(1, pawns.Count - 1);
            for (int i = 0; i < num; i++)
            {
                Pawn pawn = pawns.RandomElement();
                HediffSet set = pawn.health.hediffSet;
                BodyPartRecord part = set.GetBrain();
                pawn.health.AddHediff(HediffDefOfLocal.NeurofibromatousWorms, part);
            }

            Find.LetterStack.ReceiveLetter(def.label.Translate(), def.letterText.Translate(num, HediffDefOfLocal.NeurofibromatousWorms.label.Translate()), LetterDefOf.NegativeEvent);

            return true;
        }
    }
}
