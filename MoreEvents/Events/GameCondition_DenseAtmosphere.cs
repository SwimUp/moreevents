using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_DenseAtmosphere : GameCondition
    {
        private const int LerpTicks = 12000;

        private const float MaxTempOffset = 145f;

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 12000f, MaxTempOffset);
        }
    }
}
