using MoreEvents.Events;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul
{
    public class CaravanArrivalAction_EnterToRaidMap : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(150, 1, 150);

        public CaravanArrivalAction_EnterToRaidMap() : base()
        {

        }

        public CaravanArrivalAction_EnterToRaidMap(MapParent mapParent) : base(mapParent)
        {

        }
    }
}
