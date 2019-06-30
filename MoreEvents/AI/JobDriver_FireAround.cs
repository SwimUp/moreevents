using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.AI
{
    public class JobDriver_FireAround : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            errorOnFailed = false;

            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_General.Wait(500).WithProgressBarToilDelay(TargetIndex.A);
            Toil toil = new Toil
            {
                initAction = delegate
                {
                    IntVec3 cell = TargetLocA;
                    if (cell != IntVec3.Invalid)
                    {
                        FireUtility.TryStartFireIn(cell, Map, 0.3f);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            yield return toil;
        }
    }
}
