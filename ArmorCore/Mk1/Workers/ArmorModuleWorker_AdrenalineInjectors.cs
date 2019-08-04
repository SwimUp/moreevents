using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_AdrenalineInjectors : MKArmorModule
    {
        private int cooldown => 2800;
        private int lastFireTick = 0;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastFireTick, "lastFireTick");
        }

        public override string StatDescription()
        {
            string result = string.Format("ArmorModuleWorker_AdrenalineInjectors".Translate(), def.StatAffecter.ToArrayValues());

            return result;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Armor.Active)
            {
                yield return new Command_Action
                {
                    defaultLabel = "ArmorModuleWorker_AdrenalineInjectors_Action".Translate(),
                    icon = def.Item.uiIcon,
                    action = delegate
                    {
                        float value = Find.TickManager.TicksGame - lastFireTick;
                        if (value < cooldown)
                        {
                            Messages.Message("ArmorModuleWorker_AdrenalineInjectors_Action_CoolDown".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        Hediff firstHediffOfDef = Armor.Wearer.health.hediffSet.GetFirstHediffOfDef(HediffDefOfLocal.AdrenalinePush);
                        if (firstHediffOfDef != null)
                        {
                            firstHediffOfDef.Severity += 0.5f;

                            if (firstHediffOfDef.Severity > 0.7f && Rand.Chance(0.25f))
                            {
                                HealthUtility.AdjustSeverity(Armor.Wearer, HediffDefOfLocal.HeartAttack, 0.6f);
                            }
                        }
                        else
                        {
                            Armor.Wearer.health.AddHediff(HediffDefOfLocal.AdrenalinePush);
                        }

                        Need need = Armor.Wearer.needs.TryGetNeed(NeedDefOf.Rest);
                        if(need != null)
                        {
                            need.CurLevel += 0.04f;
                        }

                        lastFireTick = Find.TickManager.TicksGame;
                    }
                };
            }
        }
    }
}
