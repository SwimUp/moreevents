using RimOverhaul.Things;
using RimOverhaul.Things.CokeFurnace;
using RimOverhaul.Things.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimOverhaul.AI
{
    public class JobDriver_UseUsableItem: JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_General.Wait(500).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            yield return new Toil()
            {
                initAction = delegate
                {
                    UsableItem item = TargetA.Thing as UsableItem;

                    if(item != null)
                    {
                        item.Use();
                    }

                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
