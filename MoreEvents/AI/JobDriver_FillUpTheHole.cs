using RimOverhaul.Things;
using RimOverhaul.Things.CokeFurnace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimOverhaul.AI
{
    public class JobDriver_FillUpTheHole : JobDriver
    {
        public Building_TribalCrack Crack => (Building_TribalCrack)TargetThingA;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);

            Toil fillUp = new Toil()
            {
                defaultCompleteMode = ToilCompleteMode.Never
            };
            fillUp.tickAction = delegate
            {
                Pawn actor = fillUp.actor;

                if (Crack.Filled >= 100)
                {
                    Crack.Destroy();
                    Find.LetterStack.ReceiveLetter("Building_TribalCrack_Title".Translate(), "Building_TribalCrack_Desc".Translate(actor.Name.ToStringFull), LetterDefOf.PositiveEvent);

                    EndJobWith(JobCondition.Succeeded);
                    return;
                }

                SkillRecord record = actor.skills.GetSkill(SkillDefOf.Construction);
                float statValue = 0f;
                if (!record.TotallyDisabled)
                {
                    statValue = actor.GetStatValue(StatDefOf.MeleeDPS);
                    statValue *= 0.003f;
                }

                Crack.Filled += 0.00013f + statValue;
            };

            yield return fillUp;
        }
    }
}
