using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class CaravanArrivalAction_EnterToQuestMapBasic: CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(200, 1, 200);

        public CaravanArrivalAction_EnterToQuestMapBasic() : base()
        {

        }

        public CaravanArrivalAction_EnterToQuestMapBasic(MapParent mapParent) : base(mapParent)
        {

        }
    }
}
