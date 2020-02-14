using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_LowVoltageElectrifiedCladding : MKArmorModule
    {
        protected virtual float fleshDamage => Rand.Range(8, 14);
        protected virtual float fleshDischarge => -0.04f;

        protected virtual float nonFleshDamage => Rand.Range(6, 15);
        protected virtual float nonFleshDischarge => -0.08f;

        protected virtual HediffDef applyFleshHediff => null;
        protected virtual HediffDef applyNonFleshHediff => null;

        protected virtual float amountFleshHediff => 0f;

        protected virtual float amountNonFleshHediff => 0f;

        public override string StatDescription()
        {
            return "ArmorModuleWorker_LowVoltageElectrifiedCladding".Translate();
        }

        public override void CheckPreAbsorbDamage(DamageInfo dInfo, ref bool absorb)
        {
            if(Armor.EnergyCharge > 0f)
            {
                if (dInfo.Instigator is Pawn attacker)
                {
                    DoEffect(attacker, ref dInfo, ref absorb);
                }
            }
        }

        protected virtual void DoEffect(Pawn attacker, ref DamageInfo dInfo, ref bool absorb)
        {
            if (attacker.RaceProps.IsFlesh)
            {
                attacker.TakeDamage(new DamageInfo(DamageDefOf.Cut, fleshDamage));
                Armor.AddCharge(fleshDischarge);

                HediffDef hediff = applyFleshHediff;
                if(hediff != null)
                {
                    HealthUtility.AdjustSeverity(attacker, hediff, amountFleshHediff);
                }
            }
            else
            {
                attacker.TakeDamage(new DamageInfo(DamageDefOf.Stun, nonFleshDamage));
                Armor.AddCharge(nonFleshDischarge);

                HediffDef hediff = applyNonFleshHediff;
                if (hediff != null)
                {
                    HealthUtility.AdjustSeverity(attacker, hediff, amountNonFleshHediff);
                }
            }
        }
    }
}
