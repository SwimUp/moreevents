using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    [StaticConstructorOnStartup]
    public class ArmorModuleWorker_ShieldModulator : MKArmorModule
    {
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        private int maxCharge => 30;
        private float charge;
        private float chargeSpeed => 0.0008f;

        public override string StatDescription()
        {
            return "ArmorModuleWorker_ShieldModulator".Translate();
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            if(dInfo.Amount > 15)
            {
                return;
            }

            if(charge > 0)
            {
                absorb = true;

                charge = Mathf.Clamp(charge - dInfo.Amount * 0.4f, 0, charge);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action()
            {
                icon = def.Item.uiIcon,
                defaultLabel = "ArmorModuleWorker_ShieldModulator_Action".Translate(charge.ToString("f2"), maxCharge),
                defaultDesc = "ArmorModuleWorker_ShieldModulator_Action_Desc".Translate(chargeSpeed),
                action = delegate
                {

                }
            };
        }

        public override void DrawWornExtras()
        {
            if(charge > 0)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, charge);
                Vector3 vector = Armor.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                float angle = Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (charge < maxCharge)
            {
                if (Armor.Active)
                {
                    float chargeCount = Mathf.Min(chargeSpeed, Armor.EnergyCharge);
                    charge = Mathf.Clamp(charge + chargeCount, 0, maxCharge);
                    Armor.EnergyCharge = Mathf.Clamp(Armor.EnergyCharge - chargeCount, 0, Armor.EnergyCharge);
                }
            }
        }
    }
}
