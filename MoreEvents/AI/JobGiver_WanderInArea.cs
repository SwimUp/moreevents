using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.AI
{
    public class JobGiver_WanderInArea : JobGiver_Wander
    {
        public int Radius = 4;

        protected override IntVec3 GetExactWanderDest(Pawn pawn)
        {
            IntVec3 cell = pawn.mindState.duty.focus.Cell;
            if (cell == IntVec3.Invalid)
                return IntVec3.Invalid;

            return GetCell(ref cell, pawn.Map);
        }

        private IntVec3 GetCell(ref IntVec3 root, Map map) => CellFinder.RandomClosewalkCellNear(root, map, Radius, 
            (IntVec3 vec) => 
            {
                if (FireUtility.ContainsStaticFire(vec, map))
                    return false;

            return true;
        });

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            throw new NotImplementedException();
        }
    }
}
