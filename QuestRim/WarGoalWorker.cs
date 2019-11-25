using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class WarGoalWorker : IExposable
    {
        protected War war;

        public virtual void StartWar(War war)
        {
            this.war = war;
        }

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref war, "war");
        }
    }
}
