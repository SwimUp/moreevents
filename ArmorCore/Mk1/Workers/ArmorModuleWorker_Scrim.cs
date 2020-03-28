using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_Scrim : MKArmorModule
    {
        public float DodgeChance => 0.3f;

        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_Scrim".Translate());

            return result;
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            base.CheckPreAbsorbDamage(dInfo, ref absorb);

            if(Rand.Chance(DodgeChance))
            {
                absorb = true;
            }
        }
    }
}
