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
