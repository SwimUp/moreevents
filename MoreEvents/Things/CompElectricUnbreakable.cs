using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class CompElectricUnbreakable : ThingComp
    {
        public CompPowerTrader power;

        public bool HasPower
        {
            get
            {
                if (power != null && power.PowerOn)
                {
                    return !parent.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                }
                return false;
            }
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            power = parent.GetComp<CompPowerTrader>();
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);

            if (HasPower)
                absorbed = true;
        }
    }
}
