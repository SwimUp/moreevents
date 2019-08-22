using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DarkNET.Jobs
{
    public class JobDriver_GoToDarkNet : JobDriver
    {
        private Building_DarkNetConsole building => (Building_DarkNetConsole)TargetA;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return new Toil()
            {
                initAction = delegate
                {
                    building.OpenConsole(pawn);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
