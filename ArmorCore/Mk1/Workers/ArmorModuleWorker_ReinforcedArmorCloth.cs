using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_ReinforcedArmorCloth : MKArmorModule
    {
        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_ReinforcedArmorCloth".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            if(dInfo.Def == DamageDefOf.Stun)
            {
                absorb = true;
            }
        }
    }
}
