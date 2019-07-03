using RimWorld;
using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoreEvents.Events
{
    public class GameCondition_SandStorm : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["SandStorm"];

        private Map map;

        public override void Init()
        {
            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            map = Find.CurrentMap;
            WeatherDef storm = WeatherDefOfLocal.Sandstorm;
            storm.durationRange = new IntRange(Duration, Duration + 1000);
            map.weatherManager.TransitionTo(storm);
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (map.weatherManager.curWeather != WeatherDefOfLocal.Sandstorm)
            {
                WeatherDef storm = WeatherDefOfLocal.Sandstorm;
                storm.durationRange = new IntRange(Duration, Duration + 1000);
                map.weatherManager.TransitionTo(storm);
            }
        }

        public override void End()
        {
            base.End();

            map.weatherDecider.StartNextWeather();
        }
    }
}
