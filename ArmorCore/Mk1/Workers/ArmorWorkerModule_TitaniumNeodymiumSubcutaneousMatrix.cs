using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    [StaticConstructorOnStartup]
    public class ArmorWorkerModule_TitaniumNeodymiumSubcutaneousMatrix : MKArmorModule
    {
        private float powerConsumtion => -0.2f;

        private float minPowerActivate => 10;

        private int ticksForConsume => 80;

        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, Color.black);

        private bool enabled = false;

        public bool Enabled => enabled;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref enabled, "enabled");
        }

        public override string StatDescription()
        {
            return $"ArmorWorkerModule_TitaniumNeodymiumSubcutaneousMatrix".Translate();
        }

        public override void Tick()
        {
            base.Tick();

            if(enabled)
            {
                if (Find.TickManager.TicksGame % ticksForConsume == 0)
                {
                    Armor.AddCharge(powerConsumtion);
                }
            }

            if (enabled && Armor.EnergyCharge <= minPowerActivate)
                enabled = false;
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            base.CheckPreAbsorbDamage(dInfo, ref absorb);

            if (enabled)
                absorb = true;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action()
            {
                defaultLabel = "ArmorWorkerModule_TitaniumNeodymiumSubcutaneousMatrix_Action".Translate(enabled ? "T_ACTIVE".Translate() : "T_INACTIVE".Translate()),
                defaultDesc = "ArmorWorkerModule_TitaniumNeodymiumSubcutaneousMatrix_Desc".Translate(),
                icon = def.Item.uiIcon,
                action = delegate
                {
                    if(!enabled && Armor.EnergyCharge < minPowerActivate)
                    {
                        Messages.Message($"ArmorWorkerModule_TitaniumNeodymiumSubcutaneousMatrix_NotEnoughPower".Translate(minPowerActivate), MessageTypeDefOf.NeutralEvent, false);
                        return;
                    }

                    enabled = !enabled;
                }
            };
        }

        public override void DrawWornExtras()
        {
            if (enabled)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, 100);
                Vector3 vector = Armor.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                float angle = Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

    }
}
