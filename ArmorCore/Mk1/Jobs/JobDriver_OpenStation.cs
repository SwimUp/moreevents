using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents.Things.Mk1
{
    public class JobDriver_OpenStation : JobDriver
    {
        public Mk1PowerStation station;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            station = (Mk1PowerStation)TargetA.Thing;

            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return new Toil()
            {
                initAction = delegate
                {
                    station.OpenStation();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
