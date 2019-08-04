using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_FirstAidInterface : MKArmorModule
    {
        public override string StatDescription()
        {
            return $"ArmorModuleWorker_FirstAidInterface".Translate(def.HealRate);
        }
    }
}
