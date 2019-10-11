using MoreEvents.Events;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Events.DarkNetSupplyAttack
{
    public class CaravanArrivalAction_EnterToRaidMap : CaravanArrivalAction_EnterToQuestMap
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
