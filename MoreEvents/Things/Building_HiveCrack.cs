using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MoreEvents.Things
{
    public class Building_HiveCrack : Building
    {
        private bool giveBuff = false;

        private Lord lord = null;

        private List<Pawn> spawnedMobs = new List<Pawn>();
        private readonly int minSpawn = 2;
        private readonly int maxSpawn = 4;
        private readonly int spawnRate = 15000;
        private bool KingIsSpawn = false;

        private int hiveLevel = 0;
        private readonly int maxHiveLevel = 0;

        private int[] maxMobs = new int[]
        {
            10,
            15,
            20,
            25
        };

        private int[] levelHitPoints = new int[]
        {
            5000,
            4300,
            3500,
            2000
        };

        private readonly float buildingDamageMultiplier = 0.7f;

        private Job currentJob = null;

        public Building_HiveCrack()
        {
            maxHiveLevel = maxMobs.Length - 1;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref hiveLevel, "hiveLevel");
            Scribe_Values.Look(ref KingIsSpawn, "KingIsSpawn");
            Scribe_Values.Look(ref giveBuff, "giveBuff");
            Scribe_References.Look(ref lord, "lord");
            Scribe_Collections.Look(ref spawnedMobs, "spawnedMobs", LookMode.Reference);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            spawnedMobs.Clear();
        }

        public override string GetInspectString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.GetInspectString());

            int count = 0;
            foreach (var p in spawnedMobs)
                if (!p.Dead)
                    count++;

            builder.AppendLine($"{Translator.Translate("HiveCreaturesCount")}{count}");
            builder.AppendLine($"{Translator.Translate("HiveLevel")}{hiveLevel}");

            return builder.ToString();
        }

        public override void Tick()
        {
            base.Tick();

            if(!IsDay(Map))
            {
                if(!giveBuff)
                    GiveBuff();
            }
            else
            {
                if (giveBuff)
                    RemoveBuff();
            }

            if (spawnedMobs.Count < maxMobs[hiveLevel])
            {
                if(Find.TickManager.TicksGame % spawnRate == 0)
                {
                    SpawnMobs();
                }
            }
        }

        private void SpawnMobs()
        {
            if (spawnedMobs.Count >= maxMobs[hiveLevel])
                return;

            CheckList();

            int count = Rand.Range(minSpawn, maxSpawn);

            if (lord == null)
            {
                lord = LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_DefendBase(Faction.OfInsects, this.Position), Map);
            }

            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOfLocal.CaveSpelopede, Faction.OfInsects, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            for (int i = 0; i < count; i++)
            {
                if (spawnedMobs.Count >= maxMobs[hiveLevel])
                    return;

                Pawn pawn = PawnGenerator.GeneratePawn(request);
                spawnedMobs.Add(pawn);
                GenSpawn.Spawn(pawn, this.Position, Map);
                lord.AddPawn(pawn);

                if (!IsDay(Map))
                    GiveBuff(pawn);
            }
        }

        private bool IsDay(Map map)
        {
            return GenLocalDate.HourInteger(map) >= 8 && GenLocalDate.HourInteger(map) <= 20;
        }

        private void GiveBuff()
        {
            CheckList();

            giveBuff = true;
            foreach(var p in spawnedMobs)
            {
                if(!p.health.hediffSet.HasHediff(HediffDefOfLocal.CaveBettleBuff))
                {
                    p.health.AddHediff(HediffDefOfLocal.CaveBettleBuff, p.health.hediffSet.GetBrain());
                }
            }
        }
        private void GiveBuff(Pawn p)
        {
            if (!p.health.hediffSet.HasHediff(HediffDefOfLocal.CaveBettleBuff))
            {
                p.health.AddHediff(HediffDefOfLocal.CaveBettleBuff, p.health.hediffSet.GetBrain());
            }
        }

        private void RemoveBuff()
        {
            CheckList();

            giveBuff = false;
            foreach (var p in spawnedMobs)
            {
                Hediff[] hediffs = p.health.hediffSet.hediffs.Where(x => x.def == HediffDefOfLocal.CaveBettleBuff).ToArray();
                foreach(var h in hediffs)
                {
                    p.health.RemoveHediff(h);
                }
            }
        }

        private void CheckList()
        {
            int count = spawnedMobs.Count - 1;
            for(int i = count; i > 0; i--)
            {
                Pawn p = spawnedMobs[i];
                if (p.Dead)
                    spawnedMobs.RemoveAt(i);
            }
        }

        private void DefendHive(ref DamageInfo dinfo)
        {
            if (!(dinfo.Instigator is Pawn))
                return;

            if(currentJob == null)
            {
                currentJob = new Job(JobDefOf.AttackMelee, new LocalTargetInfo(dinfo.Instigator));
                currentJob.expiryInterval = 20000;
                currentJob.checkOverrideOnExpire = true;
                currentJob.expireRequiresEnemiesNearby = true;
            }

            foreach(var pawn in spawnedMobs)
            {
                if(!pawn.Dead)
                {
                    if (pawn.CurJob == currentJob)
                        continue;

                    if(pawn.Position.InHorDistOf(this.Position, 150))
                    {
                        pawn.jobs.ClearQueuedJobs();
                        pawn.jobs.StartJob(currentJob);
                    }
                }
            }
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);

            DefendHive(ref dinfo);

            if (hiveLevel != maxHiveLevel)
            {
                if (this.HitPoints <= levelHitPoints[hiveLevel])
                {
                    hiveLevel++;
                }
            }

            if(!KingIsSpawn && this.HitPoints <= 2000)
            {
                PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOfLocal.CaveBeetleKing, Faction.OfInsects, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                spawnedMobs.Add(pawn);
                GenSpawn.Spawn(pawn, this.Position, Map);
                KingIsSpawn = true;
            }

            if(Rand.Chance(0.12f))
            {
                SpawnMobs();
            }

            dinfo.SetAmount(dinfo.Amount * buildingDamageMultiplier);
        }
    }

}
