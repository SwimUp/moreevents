using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_SummerSolstice : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["SummerSolstice"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            Season season = GenLocalDate.Season(map);

            if (season != Season.Summer)
                return false;

            int day = GenLocalDate.DayOfSeason(map);
            if (day > 8 || day < 7)
            {
                return false;
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if(!pawn.Dead && pawn.RaceProps.Humanlike && pawn.RaceProps.IsFlesh)
                {
                    pawn.mindState.inspirationHandler.TryStartInspiration(InspirationDefOfLocal.Frenzy_SummerWork);

                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.PositiveCharge);
                }
            }

            SendStandardLetter();

            return true;
        }
    }
}
