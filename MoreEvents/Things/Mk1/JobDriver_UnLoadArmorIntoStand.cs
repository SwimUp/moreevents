using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.Things.Mk1
{
    public class JobDriver_UnLoadArmorIntoStand : JobDriver
    {
        public Mk1PowerStation station;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo targetA = base.job.targetA;
            Job job = base.job;
            station = (Mk1PowerStation)TargetThingA;

            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_General.Wait(400).FailOnDespawnedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);

            Toil finish = new Toil
            {
                initAction = delegate
                {
                    if(CellFinder.TryFindRandomCellNear(station.Position, station.Map, 2, null, out IntVec3 result))
                    {
                        GenSpawn.Spawn(station.ContainedArmor, result, station.Map);
                        station.ContainedArmor = null;
                    }
                    else
                    {
                        Messages.Message("NotEnoughSpace".Translate(), MessageTypeDefOf.NeutralEvent, false);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            yield return finish;
        }
    }
}
