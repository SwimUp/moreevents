using MoreEvents.Events;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Events.DarkNetCaptureMaterials
{
    public class CaravanArrivalAction_EnterToPirateCaravanMap : CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(200, 1, 200);

        public CaravanArrivalAction_EnterToPirateCaravanMap() : base()
        {

        }
        
        public CaravanArrivalAction_EnterToPirateCaravanMap(MapParent mapParent) : base(mapParent)
        {

        }
    }
}
