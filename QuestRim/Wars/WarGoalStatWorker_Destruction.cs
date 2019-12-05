using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim.Wars
{
    public class WarGoalStatWorker_Destruction : WarGoalStatWorker
    {
        public WarGoalWorker_Destruction WarGoalWorker_Destruction => (WarGoalWorker_Destruction)WarGoalWorker;

        public override void Initialize(War war)
        {
            base.Initialize(war);
        }
    }
}
