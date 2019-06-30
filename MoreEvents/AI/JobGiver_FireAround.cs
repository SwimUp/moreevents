using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.AI
{
    public class JobGiver_FireAround : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            IntVec3 cell = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 10, (IntVec3 vec) =>
            {
                if (FireUtility.ContainsStaticFire(vec, pawn.Map))
                    return false;

                return true;
            });

            Job job = new Job(JobDefOfLocal.FireAround, cell);

            return job;
        }
    }
}
