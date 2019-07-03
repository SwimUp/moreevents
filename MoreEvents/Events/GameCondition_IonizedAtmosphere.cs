using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_IonizedAtmosphere : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["IonizedAtmosphere"];

        private WeatherDef[] weathers = new WeatherDef[]
        {
            WeatherDefOfLocal.DryThunderstorm,
            WeatherDefOfLocal.RainyThunderstorm
        };

        private int cycle = 60000;
        private float cycleChance = 0.4f;
        private int cycleTimer = 0;

        public override void Init()
        {
            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            base.Init();
            cycle = Rand.Range(50000, 140000);
            cycleChance = Rand.Range(0.3f, 0.45f);

            cycleTimer = cycle;
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            cycleTimer--;

            if(cycleTimer <= 0)
            {
                cycleTimer = cycle;

                if(Rand.Chance(cycleChance))
                {
                    DoChangeWeather();
                }
            }
        }

        private void DoChangeWeather()
        {
            foreach(var map in AffectedMaps)
            {
                WeatherDef def = weathers.RandomElement();

                map.weatherManager.TransitionTo(def);
            }
        }
    }
}
