using System;
using RimWorld;

namespace MoreEvents.Events
{
    public class GameCondition_SuperHeatWave : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["SuperHeatWave"];

        private const int LerpTicks = 12000;

        private const float MaxTempOffset = 57f;

        public override void Init()
        {
            if (!settings.Active)
            {
                End();
                return;
            }

            base.Init();
        }

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 12000f, MaxTempOffset);
        }
    }
}
