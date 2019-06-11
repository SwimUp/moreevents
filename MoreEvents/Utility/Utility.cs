using System;
using System.Collections.Generic;
using RimWorld;
using System.Linq;
using Verse.Noise;
using UnityEngine;
using RimWorld.Planet;
using Verse;

namespace MoreEvents
{
    public static class Utility
    {
        private static List<List<Thing>> tempList = new List<List<Thing>>();

        public static IEnumerable<IntVec3> CellsToHit(IntVec3 center, Map map, float radius, ref List<IntVec3> openCells)
        {
            List<IntVec3> adjWallCells = new List<IntVec3>();
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && GenSight.LineOfSight(center, intVec, map, skipFirstCell: true))
                {
                    openCells.Add(intVec);
                }
            }
            for (int j = 0; j < openCells.Count; j++)
            {
                IntVec3 intVec2 = openCells[j];
                if (!intVec2.Walkable(map))
                {
                    continue;
                }
                for (int k = 0; k < 4; k++)
                {
                    IntVec3 intVec3 = intVec2 + GenAdj.CardinalDirections[k];
                    if (intVec3.InHorDistOf(center, radius) && intVec3.InBounds(map) && !intVec3.Standable(map) && intVec3.GetEdifice(map) != null && !openCells.Contains(intVec3) && adjWallCells.Contains(intVec3))
                    {
                        adjWallCells.Add(intVec3);
                    }
                }
            }
            return openCells.Concat(adjWallCells);
        }

        public static bool GenerateChance(double Chance, int iterations = 1)
        {
            System.Random r = new System.Random();

            for (int i = 0; i < iterations; i++)
            {
                double num = r.NextDouble();
                if (num < Chance)
                {
                    return true;
                }
            }

            return false;
        }

        public static int GiveHediffToRandomColinists(Map map, List<Pawn> pawnList, HediffDef hediff, int minNumber, int maxAfterCountNumber = 1)
        {
            int num = Rand.Range(minNumber, pawnList.Count - maxAfterCountNumber);
            for (int i = 0; i < num; i++)
            {
                Pawn pawn = pawnList.RandomElement();
                HediffSet set = pawn.health.hediffSet;
                BodyPartRecord part;
                List<BodyPartRecord> allParts = (from x in pawn.RaceProps.body.AllParts where !set.PartIsMissing(x) select x).ToList();
                if (allParts.TryRandomElement(out part))
                {
                    pawn.health.AddHediff(hediff, part);
                }
            }

            return num;
        }
    }

}
