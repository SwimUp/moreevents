using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events
{
    public class GameCondition_ActiveStar : GameCondition
    {
        public bool Burst = false;
        public int BurstTicks;
        public int BurstRate => 2000;
        private int burstRate = 0;
        public int BurstCount => 5;
        private int burstCount;

        private SkyColorSet Colors = new SkyColorSet(new ColorInt(242, 214, 73).ToColor, new ColorInt(242, 214, 73).ToColor, new ColorInt(242, 214, 73).ToColor, 0.95f);

        public override void Init()
        {
            base.Init();

            BurstTicks = Find.TickManager.TicksGame + (Rand.Range(2, 5) * 60000);
        }
        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (Burst)
            {
                burstRate--;

                if (burstRate <= 0)
                {
                    burstRate = BurstRate;

                    foreach (var map in AffectedMaps)
                    {
                        DoEffect(map);
                    }

                    burstCount++;

                    if (burstCount == BurstCount)
                    {
                        Burst = false;
                        BurstTicks = Find.TickManager.TicksGame + (Rand.Range(2, 5) * 60000);
                    }
                }
            }
            else
            {
                if (Find.TickManager.TicksGame >= BurstTicks)
                {
                    Burst = true;
                    burstCount = 0;
                    burstRate = BurstRate;
                }
            }
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            if(Burst)
                return new SkyTarget(0.9f, Colors, 2f, 2f);

            return null;
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 7000f, 0.8f);
        }

        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            if (Burst)
                return false;

            return base.AllowEnjoyableOutsideNow(map);
        }
        public override float TemperatureOffset()
        {
            if (Burst)
                return 170f;

            return base.TemperatureOffset();
        }
        public void DoEffect(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (CanDamage(pawn, map))
                {
                    FireUtility.TryAttachFire(pawn, 0.4f);
                }
            }

            int count = Rand.Range(4, 7);
            for(int i = 0; i < count; i++)
            {
                if(CellFinderLoose.TryGetRandomCellWith(x => !x.Roofed(map), map, 5000, out IntVec3 result))
                {
                    FireUtility.TryStartFireIn(result, map, 0.7f);
                }
            }
        }

        public bool CanDamage(Pawn pawn, Map map)
        {
            if (!pawn.def.race.IsFlesh)
                return false;

            if (pawn.Position.Roofed(map))
            {
                return false;
            }

            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Burst, "Burst");
            Scribe_Values.Look(ref burstRate, "burstRate");
            Scribe_Values.Look(ref burstCount, "burstCount");
            Scribe_Values.Look(ref BurstTicks, "BurstTicks");
        }
    }
}
