using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class CommunicationComponent_Timer : CommunicationComponent
    {
        private int ticks;

        public CommunicationComponent_Timer()
        {

        }

        public CommunicationComponent_Timer(int ticks)
        {
            this.ticks = ticks;
        }

        public override void Tick()
        {
            base.Tick();

            ticks--;

            if(ticks <= 0)
            {
                QuestsManager.Communications.RemoveComponent(this);
                TimerEnd();
            }
        }

        public virtual void TimerEnd()
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticks, "ticks");
        }
    }
}
