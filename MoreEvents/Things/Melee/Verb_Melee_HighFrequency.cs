using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.Melee
{
    public class Verb_Melee_HighFrequency : Verb_MeleeAttack
    {
        private const float MeleeDamageRandomFactorMin = 0.8f;

        private const float MeleeDamageRandomFactorMax = 1.2f;

        private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
        {
            float damAmount2 = verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
            float armorPenetration = verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
            DamageDef damDef = verbProps.meleeDamageDef;
            BodyPartGroupDef bodyPartGroupDef = null;
            HediffDef hediffDef = null;

            float additionalMultiplier = 1f;
            if(target.Thing != null)
            {
                if (target.Thing is Pawn pawn)
                {
                    if (pawn.RaceProps.IsMechanoid)
                    {
                        additionalMultiplier = 5f;
                    }
                }
                if(target.Thing is Building building)
                {
                    if (building.Stuff != null && building.Stuff.stuffProps != null)
                    {
                        StuffCategoryDef stuffCategoryDef = building.Stuff.stuffProps.categories.FirstOrDefault();
                        if (stuffCategoryDef != null)
                        {
                            if (stuffCategoryDef == StuffCategoryDefOf.Metallic)
                            {
                                additionalMultiplier = 4f;
                            }
                        }
                    }
                }
            }

            if(EquipmentSource != null)
            {
                CompChargeableWeapon compChargeableWeapon = EquipmentSource.TryGetComp<CompChargeableWeapon>();
                if(compChargeableWeapon != null)
                {
                    if (compChargeableWeapon.Charge < compChargeableWeapon.Props.ChargePerShot)
                        additionalMultiplier = 1f;
                    else
                        compChargeableWeapon.Used();
                }
            }

            damAmount2 = Rand.Range(damAmount2 * 0.8f, damAmount2 * 1.2f) * additionalMultiplier;

            if (base.CasterIsPawn)
            {
                bodyPartGroupDef = verbProps.AdjustedLinkedBodyPartsGroup(tool);
                if (damAmount2 >= 1f)
                {
                    if (base.HediffCompSource != null)
                    {
                        hediffDef = base.HediffCompSource.Def;
                    }
                }
                else
                {
                    damAmount2 = 1f;
                    damDef = DamageDefOf.Blunt;
                }
            }
            ThingDef source = (base.EquipmentSource == null) ? base.CasterPawn.def : base.EquipmentSource.def;
            Vector3 direction = (target.Thing.Position - base.CasterPawn.Position).ToVector3();
            DamageDef def = damDef;
            float amount = damAmount2;
            float armorPenetration2 = armorPenetration;
            Thing caster = base.caster;
            DamageInfo mainDinfo = new DamageInfo(def, amount, armorPenetration2, -1f, caster, null, source);
            mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
            mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
            mainDinfo.SetWeaponHediff(hediffDef);
            mainDinfo.SetAngle(direction);
            yield return mainDinfo;
            if (surpriseAttack && ((verbProps.surpriseAttack != null && !verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty()) || (tool != null && tool.surpriseAttack != null && !tool.surpriseAttack.extraMeleeDamages.NullOrEmpty())))
            {
                IEnumerable<ExtraMeleeDamage> extraDamages = Enumerable.Empty<ExtraMeleeDamage>();
                if (verbProps.surpriseAttack != null && verbProps.surpriseAttack.extraMeleeDamages != null)
                {
                    extraDamages = extraDamages.Concat(verbProps.surpriseAttack.extraMeleeDamages);
                }
                if (tool != null && tool.surpriseAttack != null && !tool.surpriseAttack.extraMeleeDamages.NullOrEmpty())
                {
                    extraDamages = extraDamages.Concat(tool.surpriseAttack.extraMeleeDamages);
                }
                foreach (ExtraMeleeDamage extraDamage in extraDamages)
                {
                    int extraDamageAmount = GenMath.RoundRandom(extraDamage.AdjustedDamageAmount(this, base.CasterPawn));
                    float extraDamageArmorPenetration = extraDamage.AdjustedArmorPenetration(this, base.CasterPawn);
                    def = extraDamage.def;
                    armorPenetration2 = extraDamageAmount;
                    amount = extraDamageArmorPenetration;
                    caster = base.caster;
                    DamageInfo extraDinfo = new DamageInfo(def, armorPenetration2, amount, -1f, caster, null, source);
                    extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                    extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                    extraDinfo.SetWeaponHediff(hediffDef);
                    extraDinfo.SetAngle(direction);
                    yield return extraDinfo;
                }
            }
        }

        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            foreach (DamageInfo item in DamageInfosToApply(target))
            {
                if (target.ThingDestroyed)
                {
                    return result;
                }
                result = target.Thing.TakeDamage(item);
            }
            return result;
        }
    }
}
