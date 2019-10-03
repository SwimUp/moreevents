using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class CompProperties_ChargeableWeapon : CompProperties
    {
        public float Capacity;

        public float ChargePerDay;

        public float ChargePerShot;

        public CompProperties_ChargeableWeapon()
        {
            compClass = typeof(CompChargeableWeapon);
        }
    }
}
