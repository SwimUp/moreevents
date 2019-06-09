using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class CaravanVisitAction_ClimateBomb : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(200, 1, 200);

        public CaravanVisitAction_ClimateBomb(MapParent mapParent) : base(mapParent)
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
