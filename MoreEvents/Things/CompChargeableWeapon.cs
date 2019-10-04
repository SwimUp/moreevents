using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things
{
    public class CompChargeableWeapon : ThingComp
    {
        public CompProperties_ChargeableWeapon Props => (CompProperties_ChargeableWeapon)props;

        public float Charge => charge;

        private float charge;

        private float chargeSpeed;

        public float ChargePerShot => Props.ChargePerShot;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            chargeSpeed = Props.ChargePerDay / 60000;
        }

        public virtual void Used()
        {
            charge = Mathf.Clamp(charge - Props.ChargePerShot, 0, Props.Capacity);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "CompChargeableWeapon_ChargeLabel".Translate(charge.ToString("f2")),
                defaultDesc = "CompChargeableWeapon_ChargeDesc".Translate(),
                action = delegate
                {

                },
                icon = parent.def.uiIcon
            };
        }

        public override string CompInspectStringExtra()
        {
            return "CompChargeableWeapon_ChargeLabel".Translate(charge.ToString("f2"));
        }

        public override string TransformLabel(string label)
        {
            return base.TransformLabel(label) + $" {"CompChargeableWeapon_ChargeLabel".Translate(charge.ToString("f2"))}";
        }

        public override void CompTick()
        {
            base.CompTick();

            if (charge == Props.Capacity)
                return;

            charge = Mathf.Clamp(charge + chargeSpeed, 0, Props.Capacity);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref charge, "Charge");
        }
    }
}
