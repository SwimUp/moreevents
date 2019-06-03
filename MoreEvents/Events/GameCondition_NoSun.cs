using UnityEngine;
using RimWorld;
using Verse;
using System;

namespace MoreEvents.Events
{
    public class GameCondition_NoSun : GameCondition
    {
        private const int LerpTicks = 12000;
        private float MinTempOffset = -200f;
        private SkyColorSet SkyColors = new SkyColorSet(new Color(0.117f, 0.117f, 0.147f), new Color(0.8f, 0.8f, 0.83f), new Color(0.3f, 0.3f, 0.6f), 1.4f);

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 12000f, MinTempOffset);
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 200f, 1f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0f, this.SkyColors, 1f, 0f));
        }
    }
}
