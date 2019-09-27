using RimOverhaul.Things;
using RimOverhaul.Things.CokeFurnace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;

namespace RimOverhaul.AI
{
    public class JobDriver_UseLaboratoryConsole : JobDriver
    {
        public Building_LaboratoryConsole Console => (Building_LaboratoryConsole)TargetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_General.Wait(2500).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            yield return new Toil()
            {
                initAction = delegate
                {
                    if(!Console.Used && Console.HasPower)
                    {
                        Console.Used = true;
                    }
                    Console.ShowHelp();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
