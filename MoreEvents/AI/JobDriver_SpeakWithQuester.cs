using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.AI
{
    public class JobDriver_SpeakWithQuester : JobDriver
    {
        private Pawn Quester => (Pawn)TargetThingA;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Quester, job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOn(() => !Quester.CanGetQuests());
            var quest = new Toil();
            quest.initAction = delegate
            {
                var actor = quest.actor;
                if (Quester.CanGetQuests())
                {
                    if(Quester.GetQuestPawn(out QuestPawn questPawn))
                    {
                        questPawn.ShowQuestDialog(actor);
                    }
                }
            };
            yield return quest;
            yield break;
        }

    }
}
