using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_AntitoxicSeals : MKArmorModule
    {
        public override bool CanAffectCondition(GameConditionDef cond)
        {
            if (!Armor.Active)
                return true;

            if (cond != GameConditionDefOf.ToxicFallout)
                return true;

            return false;
        }

        public override string StatDescription()
        {
            return "ArmorModuleWorker_AntitoxicSeals".Translate();
        }
    }
}
