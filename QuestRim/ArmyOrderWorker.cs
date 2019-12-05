using QuestRim.Wars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class ArmyOrderWorker
    {
        public ArmyOrderDef def;

        public bool CanGive(FactionArmy army, War war)
        {
            if(army.lastGiveOrderTicks.TryGetValue(def, out int value))
            {
                int daysPassed = Mathf.FloorToInt((Find.TickManager.TicksGame - war.StartTicks) / 60000f);
                if (daysPassed < def.earliestWarDay)
                    return false;

                float num = (Find.TickManager.TicksGame - value) / 60000f;
                if (num < def.minRefireDays)
                {
                    return false;
                }
            }

            if (!CanGiveSubTo(army, war))
            {
                return false;
            }

            return true;
        }

        public bool GiveTo(FactionArmy army, War war)
        {
            if (!GiveWorkerTo(army, war))
                return false;

            return true;
        }

        protected virtual bool CanGiveSubTo(FactionArmy army, War war)
        {
            return true;
        }

        protected virtual bool GiveWorkerTo(FactionArmy army, War war)
        {
            return true;
        }
    }
}
