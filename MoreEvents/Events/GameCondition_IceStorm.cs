using RimWorld;
using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoreEvents.Events
{
    public class GameCondition_IceStorm : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["IceStorm"];

        public override void Init()
        {
            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            Map map = Find.CurrentMap;
            WeatherDef storm = WeatherDefOfLocal.IncredibleSnowstorm;
            storm.durationRange = new IntRange(Duration, Duration + 1000);
            map.weatherManager.TransitionTo(storm);
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % 400 == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    this.DoPawnsFrostDamage(affectedMaps[i]);
                }
            }
        }

        private void DoPawnsFrostDamage(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (CanDamage(pawn, map))
                {
                    float num = 0.028758334f;
                    num *= pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                    if (num != 0f)
                    {
                        float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                        num *= num2;
                        BodyPartRecord bodyPartRecord;
                        HediffSet hediffSet = pawn.health.hediffSet;
                        if ((from x in pawn.RaceProps.body.AllPartsVulnerableToFrostbite
                             where !hediffSet.PartIsMissing(x)
                             select x).TryRandomElementByWeight((BodyPartRecord x) => x.def.frostbiteVulnerability, out bodyPartRecord))
                        {
                            int num5 = Mathf.CeilToInt((float)bodyPartRecord.def.hitPoints * 0.5f);
                            DamageDef frostbite = DamageDefOf.Cut;
                            float amount = (float)num5;
                            BodyPartRecord hitPart = bodyPartRecord;
                            DamageInfo dinfo = new DamageInfo(frostbite, amount, 0f, -1f, null, hitPart, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                            pawn.TakeDamage(dinfo);
                        }
                    }
                }
            }
        }

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 10000f, -60);
        }

        public bool CanDamage(Pawn pawn, Map map)
        {
            if (!pawn.def.race.IsFlesh)
                return false;

            if (pawn.Position.Roofed(map) && pawn.def.race.IsFlesh)
            {
                return false;
            }

            return true;
        }
    }
}
