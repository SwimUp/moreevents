using RimArmorCore.Mk1;
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
    public class JobDriver_SetupModuleForArmor : JobDriver
    {
        public Apparel_MkArmor armor => (Apparel_MkArmor)TargetC;
        public Thing module => TargetThingB;

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
            yield return Toils_General.Wait(1200).WithProgressBarToilDelay(TargetIndex.A).FailOnDestroyedOrNull(TargetIndex.A);

            Toil finish = new Toil
            {
                initAction = delegate
                {
                    if (module != null)
                    {
                        ArmorModuleDef moduleDef = DefDatabase<ArmorModuleDef>.AllDefs.Where(x => x.Item == module.def).FirstOrDefault();
                        if (moduleDef == null)
                            return;

                        armor.AddModule(moduleDef, module);
                        pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out Thing t);
                        module.DeSpawn();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return finish;
        }
    }
}
