using RimWorld;
using Verse;
using UnityEngine;

namespace MoreEvents.Events
{
    public class GameCondition_Endlessday : GameCondition
    {
        private EventSettings settings => Settings.EventsSettings["Endlessday"];

        private SkyColorSet DaySkyColors = new SkyColorSet(new Color(1f, 1f, 1f), new Color(0.718f, 0.745f, 0.757f), new Color(1f, 1f, 1f), 1.25f);

        public override void GameConditionTick()
        {
            if (!settings.Active)
            {
                gameConditionManager.ActiveConditions.Remove(this);
                return;
            }

            base.GameConditionTick();

            Find.CurrentMap.skyManager.ForceSetCurSkyGlow(1f);
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 200f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0f, this.DaySkyColors, 1f, 1f));
        }
    }
}
