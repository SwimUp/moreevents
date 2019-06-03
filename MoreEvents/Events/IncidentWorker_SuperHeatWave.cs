using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_SuperHeatWave : IncidentWorker_MakeGameCondition
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            Map map = (Map)parms.target;
            return map.mapTemperature.SeasonalTemp >= 60f;
            
        }
    }
}
