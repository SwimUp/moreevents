using MoreEvents.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.Events.ClimateBomb
{
    public class JobDriver_DisableClimateBomb : JobDriver
    {
        public Building_ClimateBomb bomb;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo targetA = base.job.targetA;
            bomb = (Building_ClimateBomb)TargetThingA;
            Job job = base.job;

            bomb.ShowHint();

            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref bomb, "bomb");
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil disarm = new Toil
            {
                initAction = delegate
                {

                }
            };
            disarm.tickAction = delegate
            {
                Pawn actor = disarm.actor;

                if(bomb.BlownUp)
                {
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);
                    return;
                }

                if (bomb.DisarmingProgress >= 100f)
                {
                    bomb.Disarm();
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);
                    return;
                }

                SkillRecord record = actor.skills.GetSkill(SkillDefOf.Intellectual);
                float statValue = 0f;
                if (!record.TotallyDisabled)
                {
                    statValue = actor.GetStatValue(StatDefOf.ResearchSpeed);
                    statValue *= 0.002f;
                    record.Learn(0.09f);
                }

                bomb.DisarmingProgress += bomb.DisarmingSpeed + statValue;
            };
            disarm.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            disarm.defaultCompleteMode = ToilCompleteMode.Never;

            yield return disarm;
        }
    }
}
