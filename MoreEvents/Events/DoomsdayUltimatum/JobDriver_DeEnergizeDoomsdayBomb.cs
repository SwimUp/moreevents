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
    public class JobDriver_DeEnergizeDoomsdayBomb : JobDriver
    {
        public Building_DoomsdayGun weapon => (Building_DoomsdayGun)TargetThingA;
        public bool AttackSend = false;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = base.pawn;
            LocalTargetInfo targetA = base.job.targetA;
            Job job = base.job;

            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AttackSend, "AttackSend");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil disarm = new Toil
            {
                initAction = delegate
                {
                    if (!AttackSend)
                    {
                        List<Pawn> pawns = Map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == weapon.Faction && !p.Downed && !p.Dead).ToList();

                        LordJob lordJob = new LordJob_AssaultColony(weapon.Faction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
                        Lord lord = LordMaker.MakeNewLord(weapon.Faction, lordJob, Map);
                        lord.numPawnsLostViolently = int.MaxValue;

                        foreach (var p in pawns)
                        {
                            Lord lastLord = p.GetLord();
                            if (lastLord != null)
                            {
                                if(Map.lordManager.lords.Contains(lord))
                                    Map.lordManager.RemoveLord(lastLord);
                            }

                            p.ClearMind();
                            lord.AddPawn(p);
                        }

                        AttackSend = true;
                    }
                }
            };
            disarm.tickAction = delegate
            {
                Pawn actor = disarm.actor;

                if (weapon.DeEnergizedStatus <= 0f)
                {
                    weapon.WeaponDeactivated = true;
                    weapon.TurnOffEnergoShield();
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);
                    return;
                }

                SkillRecord record = actor.skills.GetSkill(SkillDefOf.Intellectual);
                float statValue = 0f;
                if (!record.TotallyDisabled)
                {
                    statValue = actor.GetStatValue(StatDefOf.ResearchSpeed);
                    statValue *= 0.003f;
                    record.Learn(0.04f);
                }

                weapon.DeEnergizedStatus -= weapon.DeEnergizedSpeed + statValue;
            };
            disarm.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            disarm.defaultCompleteMode = ToilCompleteMode.Never;

            yield return disarm;
        }
    }
}
