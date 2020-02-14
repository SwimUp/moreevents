using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_HighVoltageElectrifiedCladding : ArmorModuleWorker_LowVoltageElectrifiedCladding
    {
        protected override float fleshDamage => Rand.Range(14, 19);
        protected override float fleshDischarge => -0.14f;

        protected override float nonFleshDamage => Rand.Range(25, 30);
        protected override float nonFleshDischarge => -0.25f;

        protected override HediffDef applyFleshHediff => MoreEvents.HediffDefOfLocal.ElectricShock;

        protected override float amountFleshHediff => 0.05f;

        public override string StatDescription()
        {
            return "ArmorModuleWorker_HighVoltageElectrifiedCladding".Translate();
        }
    }
}
