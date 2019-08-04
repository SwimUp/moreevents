using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_ReinforcedPlates : MKArmorModule
    {
        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_ReinforcedPlates".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }
    }
}
