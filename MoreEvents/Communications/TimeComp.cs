using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class TimeComp : CommunicationComponent
    {
        public CommunicationDialog Dialog;
        public int TicksToRemove = 0;

        public TimeComp()
        {

        }

        public TimeComp(CommunicationDialog dialog, int ticks)
        {
            Dialog = dialog;
            TicksToRemove = ticks;
        }

        public override void Tick()
        {
            TicksToRemove--;

            if(TicksToRemove <= 0)
            {
                RemoveNow();
            }
        }

        public void RemoveNow()
        {
            if(Dialog != null)
            {
                QuestsManager.Communications.RemoveCommunication(Dialog);
            }

            QuestsManager.Communications.RemoveComponent(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref Dialog, "Dialog");
            Scribe_Values.Look(ref TicksToRemove, "TicksToRemove");
        }
    }
}
