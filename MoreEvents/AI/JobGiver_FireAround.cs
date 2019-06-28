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
            IntVec3 cell = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 10);

            Job job = new Job(JobDefOfLocal.FireAround, cell);

            return job;
        }
    }
}
