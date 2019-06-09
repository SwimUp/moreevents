using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_DenseAtmosphere : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["DenseAtmosphere"];

        private const int LerpTicks = 12000;

        private const float MaxTempOffset = 145f;

        public override void Init()
        {
            base.Init();

            if (!settings.Active)
            {
                End();
                return;
            }
        }

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 12000f, MaxTempOffset);
        }
    }
}
