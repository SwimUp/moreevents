using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_Plato : MKArmorModule
    {
        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_Plato".Translate(), def.StatAffecter.ElementAt(0));

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
