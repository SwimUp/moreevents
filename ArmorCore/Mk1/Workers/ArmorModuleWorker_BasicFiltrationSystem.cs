using MoreEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_BasicFiltrationSystem : MKArmorModule
    {
        public override bool CanAffectCondition(GameConditionDef cond)
        {
            if (!Armor.Active)
                return true;

            if (cond == GameConditionDefOfLocal.HeavyAir)
                return false;

            return true;
        }

        public override string StatDescription()
        {
            return $"ArmorModuleWorker_BasicFiltrationSystem".Translate();
        }
    }
}
