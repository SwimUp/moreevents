using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MoreEvents.Things
{
    public class Building_DoomsdayGun : Building
    {
        public List<IntVec3> ExplosionPosition = new List<IntVec3>
        {
            new IntVec3(119, 0, 171),
            new IntVec3(112, 0, 176),
            new IntVec3(118, 0, 156),
            new IntVec3(114, 0, 152),
            new IntVec3(133, 0, 156),
            new IntVec3(137, 0, 152),
            new IntVec3(133, 0, 170),
            new IntVec3(140, 0, 176),
        };

        public bool SecuritySystemActive = true;
        public bool WeaponDeactivated = false;
        public bool EnergoShieldActive => DeEnergizedStatus > 0f;
        public float DeEnergizedStatus = 100f;
        public readonly float DeEnergizedSpeed = 0.00013f;

        public bool RaidSent = false;

        public override string GetInspectString()
        {
            string text = string.Empty;
            text += "DeEnergizedStatus".Translate(DeEnergizedStatus.ToString("f2"));
            text += SecuritySystemActive ? "SecurityActiveDoomsday".Translate() : "SecurityNonActiveDoomsday".Translate();

            return text;
        }

        public override void Tick()
        {
            base.Tick();

            if (!RaidSent && DeEnergizedStatus <= 60f)
                SendRaid();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (SecuritySystemActive)
            {
                foreach(var expPos in ExplosionPosition)
                {
                    GenExplosion.DoExplosion(expPos, Map, 55f, DamageDefOf.Bomb, null, 7000, 100);
                }

                List<Pawn> pawns = new List<Pawn>(Map.mapPawns.AllPawnsSpawned.Where(p => !p.Dead && !p.Fogged()));

                foreach (var p in pawns)
                {
                    if (!p.Dead)
                        p.TakeDamage(new DamageInfo(DamageDefOf.Bomb, Rand.Range(80, 160), 60));
                }
            }

            base.Destroy(mode);
        }

        public void SendRaid()
        {
            if (!TryFindSpawnSpot(Map, out IntVec3 spawnSpot))
            {
                return;
            }

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = Rand.Range(1800, 2400),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack
            };

            LordJob lordJob = new LordJob_AssaultColony(Faction, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
            Lord lord = LordMaker.MakeNewLord(Faction, lordJob, Map);
            lord.numPawnsLostViolently = int.MaxValue;

            IEnumerable<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
            foreach(var p in pawns)
            {
                GenSpawn.Spawn(p, spawnSpot, Map);

                lord.AddPawn(p);
            }

            Find.LetterStack.ReceiveLetter("DoomsdayContrAttackTitle".Translate(), "DoomsdayContrAttack".Translate(), LetterDefOf.ThreatBig);

            RaidSent = true;
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (EnergoShieldActive)
            {
                yield return new FloatMenuOption("DeactiveEnergyShieldDoom".Translate(), delegate
                {
                    Job job = new Job(JobDefOfLocal.DeEnergizeDoomsdayBomb, this);
                    selPawn.jobs.TryTakeOrderedJob(job);
                });
            }
            else
            {
                if(SecuritySystemActive)
                {
                    yield return new FloatMenuOption("DisableSecuritySystems".Translate(), delegate
                    {
                        Job job = new Job(JobDefOfLocal.DisableSecuritySystems, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
            }

        }

        public void TurnOffEnergoShield()
        {
            DeEnergizedStatus = 0f;

            Find.LetterStack.ReceiveLetter("ShildsOffDommsdayTitle".Translate(), "ShildsOffDommsday".Translate(), LetterDefOf.PositiveEvent);
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if(EnergoShieldActive)
            {
                absorbed = true;
                return;
            }

            absorbed = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref DeEnergizedStatus, "DeEnergizedStatus");
            Scribe_Values.Look(ref RaidSent, "RaidSent");
            Scribe_Values.Look(ref SecuritySystemActive, "SecuritySystemActive");
            Scribe_Values.Look(ref WeaponDeactivated, "WeaponDeactivated");
        }
    }
}
