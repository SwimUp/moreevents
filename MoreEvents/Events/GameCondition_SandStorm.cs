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

        public override void Init()
        {
            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            WeatherDef storm = WeatherDefOfLocal.Sandstorm;
            storm.durationRange = new IntRange(Duration, Duration + 1000);

            foreach(var map in AffectedMaps)
                map.weatherManager.TransitionTo(storm);
        }

        //public override void GameConditionTick()
        //{
        //    base.GameConditionTick();

        //    foreach (var map in AffectedMaps)
        //    {
        //        if (map.weatherManager.curWeather != WeatherDefOfLocal.Sandstorm)
        //        {
        //            WeatherDef storm = WeatherDefOfLocal.Sandstorm;
        //            storm.durationRange = new IntRange(Duration, Duration + 1000);
        //            map.weatherManager.TransitionTo(storm);
        //        }
        //    }
        //}

        public override void End()
        {
            base.End();

            foreach (var map in AffectedMaps)
                map.weatherDecider.StartNextWeather();
        }
    }
}
