using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents.Things.Mk1;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_EnergyBoots : MKArmorModule
    {
        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_EnergyBoots".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override void SetupStats(Apparel_MkArmor armor)
        {
            armor.dischargeRate += 0.10f;
        }
    }
}
