using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_LeanAtmosphere : GameCondition
    {
        private const int LerpTicks = 12000;

        private float MaxTempOffset = 70f;
        private float MinTempOffset = -120f;

        public override void Init()
        {
            this.Duration = -1;
            this.Permanent = true;
            GameCondition cond = GameConditionMaker.MakeConditionPermanent(GameConditionDefOfLocal.RadiationFon);
            Find.World.gameConditionManager.RegisterCondition(cond);
        }
        public override float TemperatureOffset()
        {
            Map map = Find.CurrentMap;

            if (GenLocalDate.HourInteger(map) >= 11 && GenLocalDate.HourInteger(map) <= 19)
                return GameConditionUtility.LerpInOutValue(this, 10000f, MaxTempOffset);
            else if(GenLocalDate.HourInteger(map) >= 20 || GenLocalDate.HourInteger(map) <= 10)
                return GameConditionUtility.LerpInOutValue(this, 10000f, MinTempOffset);
            
            return GameConditionUtility.LerpInOutValue(this, 10000f, MaxTempOffset);
        }
    }
}
