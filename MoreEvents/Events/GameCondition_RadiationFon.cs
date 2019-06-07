using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoreEvents.Events
{
    public class GameCondition_RadiationFon : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["RadiationFon"];

        private const int LerpTicks = 5000;

        private const float MaxSkyLerpFactor = 0.5f;

        private const int CheckInterval = 3451;

        private const float ToxicPerDay = 0.5f;

        private const float PlantKillChance = 0.0065f;

        private const float CorpseRotProgressAdd = 3000f;

        public override void Init()
        {
            if (!settings.Active)
            {
                End();
                return;
            }

            base.Init();
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % 3451 == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    this.DoPawnsToxicDamage(affectedMaps[i]);
                }
            }
        }

        private void DoPawnsToxicDamage(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (!pawn.Position.Roofed(map) && pawn.def.race.IsFlesh)
                {
                    float num = 0.028758334f;
                    num *= pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                    if (num != 0f)
                    {
                        float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                        num *= num2;
                        HealthUtility.AdjustSeverity(pawn, HediffDefOfLocal.Irradiation, num);
                    }
                }
            }
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
        }

        public override float AnimalDensityFactor(Map map)
        {
            return 0f;
        }

        public override float PlantDensityFactor(Map map)
        {
            return 0f;
        }

        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }

    }
}
