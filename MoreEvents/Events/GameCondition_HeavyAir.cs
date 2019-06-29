﻿using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using MoreEvents.Things.Mk1;

namespace MoreEvents.Events
{
    public class GameCondition_HeavyAir : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["HeavyAir"];

        public override void Init()
        {
            base.Init();

            if (!settings.Active)
            {
                End();
                return;
            }

            Map map = Find.CurrentMap;
            WeatherDef fog = WeatherDefOfLocal.HardFog;
            fog.durationRange = new IntRange(Duration, Duration + 1000);
            map.weatherManager.TransitionTo(fog);
        }

        public override void End()
        {
            foreach (var map in AffectedMaps)
            {
                List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawnsSpawned.Count; i++)
                {
                    Pawn pawn = allPawnsSpawned[i];

                    HealthUtility.AdjustSeverity(pawn, HediffDefOfLocal.OxygenStarvation, -100);
                }
            }

            base.End();
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % 5000 == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    this.DoPawnsAirDamage(affectedMaps[i]);
                }
            }
        }

        private void DoPawnsAirDamage(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (!pawn.Position.Roofed(map) && pawn.def.race.IsFlesh && !Apparel_Mk1.HasMk1Enable(pawn))
                {
                    float num = 0.028758334f;
                    num *= pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                    if (num != 0f)
                    {
                        float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                        num *= num2;
                        HealthUtility.AdjustSeverity(pawn, HediffDefOfLocal.OxygenStarvation, num);
                    }
                }
            }
        }
    }
}
