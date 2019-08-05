using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_SecurityInterface : MKArmorModule
    {
        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_SecurityInterface".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override void TransformStat(StatDef statDef, ref float value)
        {
            if (!Armor.Active)
                return;

            if (def.StatAffecter.ContainsKey(statDef))
            {
                value += def.StatAffecter[statDef];
            }
        }
    }
}
