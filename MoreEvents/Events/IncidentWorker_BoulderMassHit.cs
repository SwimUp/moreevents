using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace MoreEvents.Events
{
    public class IncidentWorker_BoulderMassHit : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["BoulderMassHit"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            IntVec3 intVec;

            return TryFindCell(out intVec, map);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            IntVec3 intVec;
            if (!this.TryFindCell(out intVec, map))
            {
                return false;
            }

            SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, intVec, map);

            return true;
        }
        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            int maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
            return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default(IntVec3), -1, true, true, true, true, false, false, delegate (IntVec3 x)
            {
                int num = Mathf.CeilToInt(Mathf.Sqrt((float)maxMineables)) + 2;
                CellRect cellRect = CellRect.CenteredOn(x, num, num);
                int num2 = 0;
                CellRect.CellRectIterator iterator = cellRect.GetIterator();
                while (!iterator.Done())
                {
                    if (iterator.Current.InBounds(map) && iterator.Current.Standable(map))
                    {
                        num2++;
                    }
                    iterator.MoveNext();
                }
                return num2 >= maxMineables;
            });
        }
    }
}
