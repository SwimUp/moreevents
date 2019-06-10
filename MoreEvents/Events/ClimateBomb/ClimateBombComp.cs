using MapGenerator;
using MoreEvents.MapGeneratorFactionBase;
using MoreEvents.Things;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class ClimateBombComp : WorldObjectComp
    {
        private int detonationTimer = 0;

        public Building_ClimateBomb Bomb;

        public bool BlownUp = false;

        private int[] threatsMap = new int[]
        {
            700,
            1200,
            1700,
            2400,
            4500,
            5000,
            5400
        };
        private float[] reqDefuse = new float[]
        {
            7,
            18,
            35,
            50,
            70,
            85,
            99
        };
        private int currentWave = 0;
        private int maxWaves = 7;

        public ClimateBombComp()
        {
            detonationTimer = Rand.Range(15, 30) * 60000;
        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            Bomb = GenerateBomb();
            Bomb.Site = (ClimateBombSite)parent;
        }

        public override void CompTick()
        {
            if (BlownUp)
                return;

            base.CompTick();

            if(ParentHasMap && Bomb != null)
            {
                Map map = (parent as MapParent).Map;
                if (!map.mapPawns.AnyColonistSpawned)
                    return;

                if(currentWave < maxWaves)
                {
                    if(Bomb.DisarmingProgress >= reqDefuse[currentWave])
                    {
                        NextWave();
                    }
                }
            }

            if (Bomb == null || (Bomb != null && !Bomb.Disarmed))
            {
                detonationTimer--;

                if (detonationTimer <= 0)
                {
                    Detonate();
                }
            }
        }

        private void NextWave()
        {
            Map map = Bomb.Map;

            if (!TryFindSpawnSpot(map, out IntVec3 spawnSpot))
            {
                return;
            }

            if (!TryFindEnemyFaction(out Faction enemyFac))
            {
                return;
            }

            int @int = Rand.Int;
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            raidParms.forced = true;
            raidParms.faction = enemyFac;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            raidParms.spawnCenter = spawnSpot;
            raidParms.points = threatsMap[currentWave];
            raidParms.pawnGroupMakerSeed = @int;
            var incident = new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms);
            Find.Storyteller.TryFire(incident);

            currentWave++;
        }

        private bool TryFindEnemyFaction(out Faction enemyFac)
        {
            return (from f in Find.FactionManager.AllFactions
                    where !f.def.hidden && !f.defeated && f.HostileTo(Faction.OfPlayer)
                    select f).TryRandomElement(out enemyFac);
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }

        public void Detonate()
        {
            BlownUp = true;
            ((ClimateBombSite)parent).DetonateBomb();
        }

        private Building_ClimateBomb GenerateBomb()
        {
            Map map = (parent as MapParent).Map;
            var spawnPos = map.AllCells.Where(cell => !cell.Fogged(map) && cell.Walkable(map) && cell.Standable(map) && cell.DistanceToEdge(map) > 60).RandomElement();

            if (spawnPos == null)
                return null;

            var building = (Building_ClimateBomb)GenSpawn.Spawn(ThingDefOfLocal.ClimatBomb, spawnPos, map);

            return building;
        }

        public override string CompInspectStringExtra()
        {
            string result;

            if(Bomb != null && Bomb.Disarmed)
            {
                result = $"{Translator.Translate("BombDisarmed")}";
            }
            else
            {
                result = $"{Translator.Translate("DetonationIn")}{GenDate.TicksToDays(detonationTimer).ToString("f2")}";
            }

            return result;
        }
    }
}
