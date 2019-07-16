using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    public class AddHumanlikeOrdersPatch
    {
        static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            foreach (var localTargetInfo in GenUI.TargetsAt(clickPos, TargetParms(), true))
            {
                if (!pawn.CanReach(localTargetInfo, PathEndMode.Touch, Danger.Deadly))
                {
                    opts.Add(new FloatMenuOption("CannotGoNoPath".Translate(), null));
                }
                else if (pawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
                {
                    opts.Add(new FloatMenuOption("CannotPrioritizeWorkTypeDisabled".Translate(SkillDefOf.Social.LabelCap), null));
                }
                else
                {
                    if (pawn != (Pawn)localTargetInfo.Thing)
                    {
                        opts.Add(new FloatMenuOption("TryStartQuestDialog".Translate(), delegate
                        {
                            Job job = new Job(JobDefOfLocal.SpeakWithQuester, localTargetInfo.Thing);
                            pawn.jobs.TryTakeOrderedJob(job);
                        }));
                    }
                }
            }
        }

        static TargetingParameters TargetParms()
        {
            TargetingParameters targetingParameters = new TargetingParameters();
            targetingParameters.canTargetPawns = true;
            targetingParameters.canTargetBuildings = false;
            targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
            targetingParameters.validator = delegate (TargetInfo targ)
            {
                if (!targ.HasThing)
                {
                    return false;
                }
                Pawn pawn = targ.Thing as Pawn;
                if (pawn == null)
                {
                    return false;
                }
                if (pawn.Downed)
                {
                    return false;
                }
                if(!pawn.GetQuestPawn(out QuestPawn questPawn))
                {
                    return false;
                }
                return true;
            };
            return targetingParameters;
        }
    }
}
