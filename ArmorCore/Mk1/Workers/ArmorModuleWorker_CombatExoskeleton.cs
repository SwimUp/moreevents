using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_CombatExoskeleton : MKArmorModule
    {
        private int overDriveCooldown => 10000;
        private int cost => 20;

        private int lastFireTick = 0;

        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_CombatExoskeleton".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastFireTick, "lastFireTick");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Armor.Active)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "ArmorModuleWorker_CombatExoskeleton_Action".Translate(),
                    defaultDesc = "ArmorModuleWorker_CombatExoskeleton_Action_Desc".Translate(cost),
                    icon = def.Item.uiIcon,
                    action = delegate
                    {
                        float value = Find.TickManager.TicksGame - lastFireTick;
                        if (value < overDriveCooldown)
                        {
                            Messages.Message("ArmorModuleWorker_CombatExoskeleton_Action_Cooldown".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        Hediff firstHediffOfDef = Armor.Wearer.health.hediffSet.GetFirstHediffOfDef(HediffDefOfLocal.CombatExoskeletonOverDrive);
                        if (firstHediffOfDef != null)
                        {
                            Messages.Message("ArmorModuleWorker_CombatExoskeleton_Action_Already".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        if (Armor.EnergyCharge < cost)
                        {
                            Messages.Message("ArmorModuleWorker_CombatExoskeleton_Action_Energy".Translate(cost), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        Armor.EnergyCharge = Mathf.Clamp(Armor.EnergyCharge - cost, 0, Armor.CoreComp.PowerCapacity);
                        HealthUtility.AdjustSeverity(Armor.Wearer, HediffDefOfLocal.CombatExoskeletonOverDrive, 0.5f);

                        lastFireTick = Find.TickManager.TicksGame;
                    }
                };
            }
        }
    }
}
