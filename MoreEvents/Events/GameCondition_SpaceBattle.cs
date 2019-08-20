using MoreEvents;
using MoreEvents.Events.ShipCrash;
using MoreEvents.Events.ShipCrash.Map.MapGenerator;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events
{
    public class GameCondition_SpaceBattle : GameCondition
    {
        private int ticksBetweenEvents => 14000;

        private ShipMapGenerator[] ships = new ShipMapGenerator[]
        {
            new Ship_Armory(),
            new ShipCargo_Complex(),
            new ShipCargo_Food(),
            new ShipCargo_Mining(),
            new Ship_Living()
        };


        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if(Find.TickManager.TicksGame % ticksBetweenEvents == 0)
            {
                DoBattleShot();
            }
        }

        private void DoBattleShot()
        {
            if (Rand.Chance(0.75f))
            {
                DoRandomShot(Rand.Range(2, 6));
            }

            if (Rand.Chance(0.06f))
            {
                DoShipFall();
            }

            if(Rand.Chance(0.12f))
            {
                DoMapPartDrop();
            }
        }

        private void DoShipFall()
        {
            ShipMapGenerator generator = ships[Rand.Range(0, ships.Length)];

            int tile = TileFinder.RandomStartingTile();
            ShipCrash_Controller.MakeShipPart(generator, tile, Find.FactionManager.AllFactionsVisible.Where(x => x != Faction.OfPlayer).RandomElement());

            Messages.Message("GameCondition_SpaceBattleTitle".Translate(), MessageTypeDefOf.NeutralEvent, false);
        }

        private void DoMapPartDrop()
        {
            IncidentDef def = IncidentDefOf.ShipChunkDrop;
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(def.category, Find.AnyPlayerHomeMap);

            def.Worker.TryExecute(parms);
        }

        private void DoRandomShot(int count)
        {
            for(int i = 0; i < AffectedMaps.Count; i++)
            {
                Map map = AffectedMaps[i];

                for (int i2 = 0; i2 < count; i2++)
                {
                    IntVec3 cell = map.AllCells.Where(c => !c.Fogged(map) && c.GetRoof(map) != RoofDefOf.RoofRockThick && !c.CloseToEdge(map, 13)).RandomElement();
                    TryFindSpawnSpot(map, out IntVec3 spawnSpot);
                    Projectile proj = (Projectile)GenSpawn.Spawn(ThingDefOfLocal.Bullet_Shell_HighExplosive, spawnSpot, map);
                    proj.Launch(null, proj.DrawPos, cell, cell, hitFlags: ProjectileHitFlags.All);
                }
            }
        }

        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }
    }
}
