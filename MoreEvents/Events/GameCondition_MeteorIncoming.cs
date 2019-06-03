using System;
using RimWorld;
using Verse;

namespace MoreEvents.Events
{
    public class GameCondition_MeteorIncoming : GameCondition
    {
        private int _maxMeteor;
        private int _curMeteor = 0;
        private int _waitTick = 150;
        private int _ticks = 0;

        public override void Init()
        {
            int max = (int)(Duration * 0.001f);
            if (max >= 900)
            {
                _maxMeteor = Rand.Range(300, max) * 3;
            } else if (max >= 500)
            {
                _maxMeteor = Rand.Range(120, max) * 3;
            }
            else
            {
                _maxMeteor = Rand.Range(40, max) * 2;
            }
            _ticks = _waitTick; 
        }
        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (_maxMeteor == _curMeteor)
            {
                End();
            }
            if (_ticks == 0)
            {
                IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.Maps.Find((Map x) => x.IsPlayerHome));
                IncidentDefOfLocal.BoulderMassHit.Worker.TryExecute(parms);
                _curMeteor++;
                _ticks = _waitTick;
            }
            _ticks--;

        }
    }
}
