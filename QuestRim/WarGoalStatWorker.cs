using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class WarGoalStatWorker : IExposable
    {
        protected War war;

        protected WarGoalWorker WarGoalWorker => war.Worker;

        public WarGoalStatWorker()
        {

        }

        public virtual string GetStat()
        {
            return "";
        }

        public virtual void Initialize(War war)
        {
            this.war = war;
        }

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref war, "war");
        }
    }
}
