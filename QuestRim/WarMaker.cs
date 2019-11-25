using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public static class WarMaker
    {
        public static War MakeWar(string warName, WarGoalDef goal, FactionInteraction declaredWarFaction, FactionInteraction defendingFaction)
        {
            War war = new War(warName, goal, declaredWarFaction, defendingFaction);
            war.id = QuestsManager.Communications.UniqueIdManager.GetNextWarID();

            return war;
        }
    }
}
