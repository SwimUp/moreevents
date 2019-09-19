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
    public class JobDriver_RefuelArmorCore : JobDriver
    {
        public Mk1PowerStation station => (Mk1PowerStation)TargetThingA;
        public Thing item => TargetThingB;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo target = base.job.targetA;
            Job job = base.job;

            int result;
            if(pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                pawn = base.pawn;
                target = job.GetTarget(TargetIndex.B).Thing;
                job = base.job;

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
            yield return Toils_General.Wait(1500).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);

            Toil finish = new Toil
            {
                initAction = delegate
                {
                    if (item != null)
                    {
                        Apparel_MkArmor mk1 = station.ContainedArmor;
                        if (mk1 != null)
                        {
                            int toRefuel = (int)(mk1.CoreComp.Props.MaxFuel - mk1.CoreComp.Fuel);
                            int num = Mathf.Min(toRefuel, item.stackCount);
                            item.SplitOff(num).Destroy();
                            mk1.CoreComp.AddFuel(num);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            yield return finish;
        }
    }
}
