using RimWorld;
using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MoreEvents.Weather;

namespace MoreEvents.Events
{
    public class GameCondition_SandStorm : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["SandStorm"];

        private SkyColorSet Colors = new SkyColorSet(new ColorInt(171, 139, 75).ToColor, new ColorInt(153, 147, 147).ToColor, new ColorInt(171, 139, 75).ToColor, 0.95f);

        private List<SkyOverlay> overlays = new List<SkyOverlay>
        {
            new WeatherOverlay_Sandstorm()
        };

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(0.9f, Colors, 1f, 1f);
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 3000f, 0.5f);
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return overlays;
        }

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

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            for (int j = 0; j < overlays.Count; j++)
            {
                for (int k = 0; k < affectedMaps.Count; k++)
                {
                    overlays[j].TickOverlay(affectedMaps[k]);
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].DrawOverlay(map);
            }
        }

        public override void End()
        {
            base.End();

            foreach (var map in AffectedMaps)
                map.weatherDecider.StartNextWeather();
        }
    }
}
