using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Weapons
{
    public class Verb_ChargableWeaponShoot : Verb_Shoot
    {
        public CompChargeableWeapon CompChargeableWeapon
        {
            get
            {
                if (compChargeableWeapon == null)
                {
                    if (EquipmentSource != null)
                    {
                        compChargeableWeapon = EquipmentSource.GetComp<CompChargeableWeapon>();
                    }
                }

                return compChargeableWeapon;
            }
        }
        private CompChargeableWeapon compChargeableWeapon;

        public override bool Available()
        {
            if(CompChargeableWeapon != null)
            {
                if (CompChargeableWeapon.Charge < CompChargeableWeapon.ChargePerShot)
                    return false;
            }

            return base.Available();
        }

        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            if (flag && base.CasterIsPawn)
            {
                if(CompChargeableWeapon != null)
                {
                    CompChargeableWeapon.Used();
                }

                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
