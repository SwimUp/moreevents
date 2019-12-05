using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class ArmyOrderDef : Def
    {
        public List<WarGoalDef> targetGoals;

        public Type workerClass;

        public ArmyOrderWorker Worker
        {
            get
            {
                if(workerInt == null)
                {
                    workerInt = (ArmyOrderWorker)Activator.CreateInstance(workerClass);
                    workerInt.def = this;
                }

                return workerInt;
            }
        }

        [Unsaved]
        private ArmyOrderWorker workerInt;

        public int minRefireDays;

        public int earliestWarDay;
    }
}
