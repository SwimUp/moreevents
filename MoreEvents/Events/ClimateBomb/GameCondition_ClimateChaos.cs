using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class GameCondition_ClimateChaos : GameCondition
    {
        private int cycle = 8000;
        private int cycleTimer = 0;

        private List<WeatherDef> weathers => DefDatabase<WeatherDef>.AllDefsListForReading;
        private List<GameConditionDef> incidents = new List<GameConditionDef>()
        {
            GameConditionDefOfLocal.HeatWave,
            GameConditionDefOfLocal.Flashstorm,
            GameConditionDefOfLocal.SandStorm,
            GameConditionDefOfLocal.ColdSnap
        };

        public override void Init()
        {
            base.Init();

            cycleTimer = cycle;
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            cycleTimer--;

            if (cycleTimer <= 0f)
            {
                cycleTimer = cycle;
                DoChange();
            }
        }


        private void DoChange()
        {
            if (Rand.Chance(0.3f))
            {
                ChangeWeather();
                Log.Message($"DO WEATHER");
            }
            else
            {
                ChangeIncident();
                Log.Message($"DO INCIDENT");
            }
        }

        private void ChangeWeather()
        {
            WeatherDef def = weathers.RandomElement();

            foreach(var map in AffectedMaps)
                map.weatherManager.TransitionTo(def);
        }

        private void ChangeIncident()
        {
            GameConditionDef def = incidents.RandomElement();

            foreach (var map in AffectedMaps)
                map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(def, Rand.Range(4000, 18000)));
        }
    }
}
