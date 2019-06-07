using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_SuperHeatWave : IncidentWorker_MakeGameCondition
    {
        private EventSettings settings => Settings.EventsSettings["SuperHeatWave"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            Map map = (Map)parms.target;
            return map.mapTemperature.SeasonalTemp >= 60f;
            
        }
    }
}
