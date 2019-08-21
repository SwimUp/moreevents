using RimOverhaul.Things.CokeFurnace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimOverhaul.AI
{
    public class JobDriver_CarryItemToFurnace : JobDriver
    {
        public Building_CokeFurnace furnace => (Building_CokeFurnace)TargetThingA;
        public Thing item => TargetThingB;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo target = base.job.GetTarget(TargetIndex.A);
            Job job = base.job;
            bool errorOnFailed2 = errorOnFailed;
            if (!pawn.Reserve(target, job, 1, -1, null, errorOnFailed2))
            {
                return false;
            }
            base.pawn.ReserveAsManyAsPossible(base.job.GetTargetQueue(TargetIndex.B), base.job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
            yield return extract;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, true, false, true);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_General.Wait(400).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            Toil finish = new Toil
            {
                initAction = delegate
                {
                    if (item != null)
                    {
                        furnace.AddItemToFurnace(item);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return finish;
            yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);

        }
    }
}
