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
    public class Dangerous_HiddenAttack : WorldObjectComp
    {
        private int ticks = 0;
        private bool startTimer = false;

        private float points;
        private Faction faction;
        private Verse.Map map;

        private int maxPawns = -1;

        public Dangerous_HiddenAttack(int ticksBeforeAttack, float threatPoints, Faction faction, Verse.Map map, int maxPawns = -1)
        {
            ticks = ticksBeforeAttack;
            startTimer = true;

            this.points = threatPoints;
            this.faction = faction;
            this.map = map;

            this.maxPawns = maxPawns;
        }

        public override void CompTick()
        {
            if (startTimer)
            {
                ticks--;

                if (ticks <= 0)
                {
                    startTimer = false;
                    SpawnEnemies();
                }
            }
        }

        private void SpawnEnemies()
        {
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

            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out IntVec3 startingSpot, out IntVec3 second);

            int max = maxPawns == -1 ? pawns.Count : maxPawns;
            if (max > pawns.Count)
                max = pawns.Count;

            for(int i = 0; i < max; i++)
            {
                Pawn p = pawns[i];
                GenSpawn.Spawn(p, startingSpot, map);
            }

            LordJob lordJob = new LordJob_AssaultColony(faction, canKidnap: true, canTimeoutOrFlee: false);
            if (lordJob != null)
            {
                LordMaker.MakeNewLord(faction, lordJob, map, pawns);
            }

            pawns.Clear();
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref ticks, "ticks", 0);
        }
    }
}
