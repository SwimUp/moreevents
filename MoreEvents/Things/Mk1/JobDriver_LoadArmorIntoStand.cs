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
    public class JobDriver_LoadArmorIntoStand : JobDriver
    {
        public Mk1PowerStation station;
        public Apparel_Mk1 armor;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo target = base.job.targetA;
            Job job = base.job;

            station = (Mk1PowerStation)TargetThingA;
            armor = (Apparel_Mk1)TargetThingB;

            int result;
            if(pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                pawn = base.pawn;
                target = job.GetTarget(TargetIndex.B).Thing;
                job = base.job;

                station = (Mk1PowerStation)TargetThingA;
                armor = (Apparel_Mk1)TargetThingB;

                result = (pawn.Reserve(target, job, 1, -1, null) ? 1 : 0);
            }
            else
            {
                result = 0;
            }

            return result != 0;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_General.Wait(600).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            Toil finish = new Toil
            {
                initAction = delegate
                {
                    if (armor != null)
                    {
                        station.ContainedArmor = armor;
                        pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out Thing t);
                        armor.DeSpawn();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return finish;
        }
    }
}
