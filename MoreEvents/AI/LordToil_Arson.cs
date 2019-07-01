using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MoreEvents.AI
{
    public class LordToil_Arson : LordToil
    {
        private IntVec3 spot;

        public LordToil_Arson(IntVec3 spot)
        {
            this.spot = spot;
        }

        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                lord.ownedPawns[i].mindState.duty = new PawnDuty(DutyDefOfLocal.Arson, spot);
            }
        }

        public override void LordToilTick()
        {
            base.LordToilTick();

            if(lord.ticksInToil % 5000  == 0)
            {
                CellFinder.TryFindRandomCellNear(spot, Map, 35, (IntVec3 val) => val.Walkable(Map) && val.DistanceToEdge(Map) > 10, out spot);
                UpdateAllDuties();
            }
        }
    }
}
