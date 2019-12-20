using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.AI
{
    public static class CaravanUtility
    {
        public static void KillRandomPawns(Caravan caravan, float attackPower, ref DamageInfo damageInfo)
        {
            List<Pawn> pawns = new List<Pawn>(caravan.PawnsListForReading);

            foreach(var pawn in pawns)
            {
                try
                {
                    int shootsCount = Rand.Range(0, 4);
                    for (int i = 0; i < shootsCount; i++)
                    {
                        if (pawn.Dead)
                        {
                            caravan.RemovePawn(pawn);
                            break;
                        }

                        pawn.TakeDamage(damageInfo);
                    }
                }
                catch
                {

                }
            }
        }

        public static void KillRandomPawns(Caravan caravan, float attackPower)
        {
            List<Pawn> pawns = new List<Pawn>(caravan.PawnsListForReading);
            List<DamageInfo> damageList = new List<DamageInfo>
            {
                new DamageInfo(DamageDefOf.Bullet, attackPower),
                new DamageInfo(DamageDefOf.Blunt, attackPower),
                new DamageInfo(DamageDefOf.Crush, attackPower),
                new DamageInfo(DamageDefOf.Cut, attackPower),
                new DamageInfo(DamageDefOf.Stab, attackPower)
            };
            
            foreach (var pawn in pawns)
            {
                int shootsCount = Rand.Range(0, 4);
                for (int i = 0; i < shootsCount; i++)
                {
                    if (pawn.Dead)
                    {
                        caravan.RemovePawn(pawn);
                        break;
                    }

                    pawn.TakeDamage(damageList.RandomElement());
                }
            }
        }

        public static void CheckIfCaravanOverWeight(Caravan caravan)
        {
            if (CaravanOverWeight(caravan))
            {
                List<Thing> items = CaravanInventoryUtility.AllInventoryItems(caravan);

                for (int i = 0; i < 1000; i++)
                {
                    if (items.Count == 0)
                        return;

                    if (!CaravanOverWeight(caravan))
                        return;

                    items.RemoveLast();
                }
            }
        }

        public static bool CaravanOverWeight(Caravan caravan) => caravan.MassCapacity <= caravan.MassUsage;
    }
}
