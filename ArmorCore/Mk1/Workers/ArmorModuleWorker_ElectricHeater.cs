using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents.Things.Mk1;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_ElectricHeater : MKArmorModule
    {
        public override string StatDescription()
        {
            return "ArmorModuleWorker_ElectricHeater".Translate();
        }

        public override void SetupStats(Apparel_MkArmor armor)
        {
            armor.dischargeRate += 0.07f;
        }
    }
}
