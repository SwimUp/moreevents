using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.SiegeCamp
{
    public class CaravanVisitAction_SiegeCamp : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(180, 1, 180);

        public CaravanVisitAction_SiegeCamp(MapParent mapParent) : base(mapParent)
        {
        }

        public override FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
        }
    }
}
