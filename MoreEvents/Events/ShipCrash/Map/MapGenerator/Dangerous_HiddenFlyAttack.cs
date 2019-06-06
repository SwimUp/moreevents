using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events.ShipCrash.Map.MapGenerator
{
    public class Dangerous_HiddenFlyAttack : WorldObjectComp
    {
        private int ticks = 0;
        private bool startTimer = false;

        private float points;
        private Faction faction;
        private Verse.Map map;

        private int maxPawns = -1;

        private int minPods;
        private int maxPods;

        public Dangerous_HiddenFlyAttack(int ticksBeforeAttack, float threatPoints, Faction faction, Verse.Map map, int minPods, int maxPods, int maxPawns = -1)
        {
            ticks = ticksBeforeAttack;
            startTimer = true;

            this.points = threatPoints;
            this.faction = faction;
            this.map = map;

            this.maxPawns = maxPawns;

            this.minPods = minPods;
            this.maxPods = maxPods;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            ForceSpawn();
        }

        public override void CompTick()
        {
            if (startTimer)
            {
                ticks--;

                if (ticks <= 0)
                {
                    SpawnEnemies();
                }
            }
        }

        public void ForceSpawn() => SpawnEnemies();

        private void SpawnEnemies()
        {
            startTimer = false;

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = faction,
                points = points,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            int podsCount = Rand.Range(minPods, maxPods);
            int pawnsCount = maxPawns == -1 ? pawns.Count : maxPawns;

            if (pawnsCount >= pawns.Count)
                pawnsCount = pawns.Count - 1;

            int pawnsInPod = pawnsCount / podsCount;
            if (pawnsInPod < 1)
                pawnsInPod = pawnsCount;

            for(int i = 0; i < podsCount; i++)
            {
                if (pawnsCount == 0)
                    break;

                IntVec3 pos = map.AllCells.Where(vec => !vec.Fogged(map) && vec.Standable(map)).RandomElement();
                if (pos == null)
                    continue;

                List<Thing> pawnsToDrop = new List<Thing>();
                for (int i2 = 0; i2 < pawnsInPod; i2++)
                {
                    if (pawnsCount == 0)
                        break;

                    pawnsToDrop.Add(pawns[pawnsCount]);
                    pawnsCount--;
                }

                DropPodUtility.DropThingsNear(pos, map, pawnsToDrop);
            }

            LordJob lordJob = new LordJob_AssaultColony(faction, canKidnap: true, canTimeoutOrFlee: false);
            if (lordJob != null)
            {
                LordMaker.MakeNewLord(faction, lordJob, map, pawns);
            }

            pawns.Clear();
        }
    }
}
