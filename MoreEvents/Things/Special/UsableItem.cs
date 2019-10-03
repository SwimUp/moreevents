using MoreEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimOverhaul.Things.Special
{
    public abstract class UsableItem : ThingWithComps
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("RimOverhaul_UseCommand".Translate(), delegate
            {
                if (selPawn.CanReserveAndReach(this, PathEndMode.Touch, Danger.Deadly))
                {
                    TryTakeJob(selPawn);
                }
            });
        }

        public virtual void TryTakeJob(Pawn pawn)
        {
            Job job = new Job(JobDefOfLocal.UseUsableItem, this);
            pawn.jobs.TryTakeOrderedJob(job);
        }

        public abstract void Use();
    }
}
