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
    public class JobDriver_CarryReactorToStation : JobDriver
    {
        public Mk1PowerStation station => (Mk1PowerStation)TargetThingA;
        public Thing core => TargetThingB;

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
                    if (core != null)
                    {
                        Apparel_MkArmor mk1 = station.ContainedArmor as Apparel_MkArmor;
                        if (mk1 != null)
                        {
                            if (mk1.Core != null)
                            {
                                CellFinder.TryFindRandomCellNear(station.Position, station.Map, 2, null, out IntVec3 result);
                                GenSpawn.Spawn(mk1.Core, result, station.Map);
                                mk1.Core = null;
                            }

                            mk1.ChangeCore(core);
                            pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out Thing t);
                            core.DeSpawn();
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return finish;
        }
    }
}
