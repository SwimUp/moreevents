﻿using MapGenerator;
using MoreEvents.MapGeneratorFactionBase;
using MoreEvents.Sounds;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.SiegeCamp
{
    public class SiegeCampSiteComp : WorldObjectComp
    {
        private BaseBlueprintDef[] level1Generators = new BaseBlueprintDef[]
        {
                    BlueprintDefOfLocal.SiegeCampBase1_1,
                    BlueprintDefOfLocal.SiegeCampBase1_2,
                    BlueprintDefOfLocal.SiegeCampBase1_3,
                    BlueprintDefOfLocal.SiegeCampBase1_4,
                    BlueprintDefOfLocal.SiegeCampBase1_5
        };
        private BaseBlueprintDef[] level2Generators = new BaseBlueprintDef[]
        {
                    BlueprintDefOfLocal.SiegeCampBase2_1,
                    BlueprintDefOfLocal.SiegeCampBase2_2,
                    BlueprintDefOfLocal.SiegeCampBase2_3,
                    BlueprintDefOfLocal.SiegeCampBase2_4,
                    BlueprintDefOfLocal.SiegeCampBase2_5
        };
        private BaseBlueprintDef[] level3Generators = new BaseBlueprintDef[]
        {
                    BlueprintDefOfLocal.SiegeCampBase3_1,
                    BlueprintDefOfLocal.SiegeCampBase3_2,
                    BlueprintDefOfLocal.SiegeCampBase3_3
        };
        private int[] levels = new int[]
        {
                Rand.Range(0, 5),
                Rand.Range(0, 5),
                Rand.Range(0, 3)
        };
        private int siegeCampLevel = 0;
        private readonly int maxLevel = 2;
        private BaseBlueprintDef baseBlueprint
        {
            get
            {
                int genInt = levels[siegeCampLevel];

                switch(siegeCampLevel)
                {
                    case 0:
                        return level1Generators[genInt];
                    case 1:
                        return level2Generators[genInt];
                    case 2:
                        return level2Generators[genInt];
                }

                return null;
            }
        }
        private readonly int ticksBetweenUpdate = 5 * 60000;
        private int timer = 0;
        private bool enable = false;

        private float totalThreat = 0;

        private int raidTimer = 0;
        private int ticksBetweenRaids = 3 * 60000;

        private int mortalShellingTimer = 0;
        private int ticksBetweenMortalShelling = 3 * 60000;
        private bool mortarsFiring = false;
        private int mortarsBulletCount = 0;
        private int mortarShellTimer = 0;
        private int ticksBetweenShots = 150;

        private SiegeCampSite camp;

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);

            timer = ticksBetweenUpdate;
            raidTimer = ticksBetweenRaids;
            mortalShellingTimer = ticksBetweenMortalShelling;

            int count = 1;
            foreach (var pawnData in baseBlueprint.pawnLegend.Values)
            {
                if (pawnData.Count.max > 1)
                    count = pawnData.Count.min;
                else
                    count = 1;

                totalThreat += pawnData.Kind.combatPower * count;
            }

            totalThreat *= 0.7f;

            camp = (SiegeCampSite)parent;

            enable = true;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref timer, "timer");
            Scribe_Values.Look(ref enable, "enable");

            Scribe_Values.Look(ref totalThreat, "totalThreat");

            Scribe_Values.Look(ref raidTimer, "raidTimer");

            Scribe_Values.Look(ref mortalShellingTimer, "mortalShellingTimer");
            Scribe_Values.Look(ref mortarsFiring, "mortarsFiring");
            Scribe_Values.Look(ref mortarsBulletCount, "mortarsBulletCount");
            Scribe_Values.Look(ref mortarShellTimer, "mortarShellTimer");

            Scribe_Values.Look(ref siegeCampLevel, "siegeCampLevel");
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Translator.Translate("SiegeCampLevel")} {siegeCampLevel + 1}");
            if (siegeCampLevel < maxLevel)
                builder.Append($"{Translator.Translate("SiegeCampUpdateTimer")} {GenDate.TicksToDays(timer).ToString("f2")}");

            return builder.ToString();
        }

        public void Stop() => enable = false;
        public void Start() => enable = true;

        public override void CompTick()
        {
            base.CompTick();

            try
            {
                if (enable)
                {
                    if (siegeCampLevel < maxLevel)
                    {
                        timer--;

                        if (timer <= 0)
                        {
                            LevelUpCamp();
                        }
                    }

                    raidTimer--;
                    if (raidTimer <= 0)
                    {
                        SendRaid();
                    }

                    if (mortarsFiring)
                    {
                        if (camp.PlayerSiegeMap == null)
                            return;

                        if (mortarsBulletCount == 0)
                            mortarsFiring = false;

                        mortarShellTimer--;

                        if (mortarShellTimer <= 0)
                        {
                            IntVec3 cell = camp.PlayerSiegeMap.AllCells.Where(c => !c.Fogged(camp.PlayerSiegeMap) && c.GetRoof(camp.PlayerSiegeMap) != RoofDefOf.RoofRockThick && !c.CloseToEdge(camp.PlayerSiegeMap, 13)).RandomElement();
                            TryFindSpawnSpot(camp.PlayerSiegeMap, out IntVec3 spawnSpot);
                            Projectile proj = (Projectile)GenSpawn.Spawn(ThingDefOfLocal.Bullet_Shell_HighExplosive, spawnSpot, camp.PlayerSiegeMap);
                            proj.Launch(null, proj.DrawPos, cell, cell, hitFlags: ProjectileHitFlags.All);
                            mortarsBulletCount--;
                            mortarShellTimer = ticksBetweenShots;
                        }
                    }
                    else
                    {
                        mortalShellingTimer--;
                        if (mortalShellingTimer <= 0)
                        {
                            SendMortalShelling();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void UpdateCamp()
        {
            var letter = LetterMaker.MakeLetter("YourAttackWasRepelledTitle".Translate(), "YourAttackWasRepelledDesc".Translate(), LetterDefOf.NeutralEvent);
            Find.LetterStack.ReceiveLetter(letter);

            if (Rand.Chance(0.25f))
                RerollMaps();

            Current.Game.DeinitAndRemoveMap(camp.Map);
        }

        private void RerollMaps()
        {
            levels[0] = Rand.Range(0, 5);
            levels[1] = Rand.Range(0, 5);
            levels[2] = Rand.Range(0, 3);
        }

        public void SendRaid()
        {
            raidTimer = ticksBetweenRaids;

            if (camp.PlayerSiegeMap == null)
                return;

            if (!TryFindSpawnSpot(camp.PlayerSiegeMap, out IntVec3 spawnSpot))
                return;

            float points = totalThreat * 0.5f;
            if (totalThreat <= 0)
                points = totalThreat;

            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, camp.PlayerSiegeMap);
            raidParms.forced = true;
            raidParms.faction = camp.Faction;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            raidParms.spawnCenter = spawnSpot;
            raidParms.points = points;
            raidParms.pawnGroupMakerSeed = Rand.Int;
            var incident = new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms);
            Find.Storyteller.TryFire(incident);
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }

        public void SendMortalShelling()
        {
            mortalShellingTimer = ticksBetweenMortalShelling;

            mortarsBulletCount = Rand.Range(7, 13) * (siegeCampLevel + 1);
            mortarsFiring = true;
            mortarShellTimer = ticksBetweenShots;
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();
                       
            BlueprintHandler.CreateBlueprintAt(camp.Map.Center, camp.Map, baseBlueprint, camp.Faction, null, out Dictionary<Pawn, LordType> pawnsList, out totalThreat, true);
        }

        private void LevelUpCamp()
        {
            timer = ticksBetweenUpdate;
            siegeCampLevel++;

            if(camp.HasMap)
            {
                Current.Game.DeinitAndRemoveMap(camp.Map);
            }
        }
    }
}
