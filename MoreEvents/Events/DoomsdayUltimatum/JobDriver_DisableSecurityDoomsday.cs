using MoreEvents.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class JobDriver_DisableSecurityDoomsday : JobDriver
    {
        public Building_DoomsdayGun weapon;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo targetA = base.job.targetA;
            weapon = (Building_DoomsdayGun)TargetThingA;
            Job job = base.job;

            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref weapon, "weapon");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil disarm = new Toil
            {
                initAction = delegate
                {
                    Find.WindowStack.Add(new Doomsday_SecurityTerminal(TerminalDefOfLocal.DoomsdaySecurityTerminal, weapon));
                }
            };
            disarm.defaultCompleteMode = ToilCompleteMode.Instant;

            yield return disarm;
        }
    }
}
