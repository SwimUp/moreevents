using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorWorkerModule_MedicalExoskeleton : MKArmorModule
    {
        private int cooldown => 35000;
        private int lastFireTick;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastFireTick, "lastFireTicks");
        }

        public override string StatDescription()
        {
            return $"ArmorWorkerModule_MedicalExoskeleton".Translate(def.HealRate);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action()
            {
                defaultLabel = "ArmorWorkerModule_MedicalExoskeleton_Action".Translate(),
                defaultDesc = "ArmorWorkerModule_MedicalExoskeleton_Action_Desc".Translate(),
                icon = def.Item.uiIcon,
                action = delegate
                {
                    float value = Find.TickManager.TicksGame - lastFireTick;
                    if (value < cooldown)
                    {
                        Messages.Message("ArmorWorkerModule_MedicalExoskeleton_Action_Cooldown".Translate(), MessageTypeDefOf.NeutralEvent, false);
                        return;
                    }

                    Hediff firstHediffOfDef = Armor.Wearer.health.hediffSet.GetFirstHediffOfDef(HediffDefOfLocal.MedicalExoskeletonOverdose);
                    if (firstHediffOfDef != null)
                    {
                        Messages.Message("ArmorWorkerModule_MedicalExoskeleton_Action_Already".Translate(), MessageTypeDefOf.NeutralEvent, false);
                        return;
                    }

                    HealthUtility.AdjustSeverity(Armor.Wearer, HediffDefOfLocal.MedicalExoskeletonOverdose, 0.5f);

                    lastFireTick = Find.TickManager.TicksGame;
                }
            };
        }
    }
}
