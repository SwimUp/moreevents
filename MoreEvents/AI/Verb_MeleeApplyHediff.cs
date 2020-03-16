using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.AI
{
    public class Verb_MeleeApplyHediff : Verb_MeleeAttack
    {
        public override bool IsUsableOn(Thing target)
        {
            return target is Pawn;
        }

        private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
        {
            float damAmount2 = verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
            float armorPenetration = verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
            DamageDef damDef = verbProps.meleeDamageDef;
            BodyPartGroupDef bodyPartGroupDef = null;
            HediffDef hediffDef = null;
            damAmount2 = Rand.Range(damAmount2 * 0.8f, damAmount2 * 1.2f);
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
                IEnumerable<ExtraDamage> extraDamages = Enumerable.Empty<ExtraDamage>();
                if (verbProps.surpriseAttack != null && verbProps.surpriseAttack.extraMeleeDamages != null)
                {
                    extraDamages = extraDamages.Concat(verbProps.surpriseAttack.extraMeleeDamages);
                }
                if (tool != null && tool.surpriseAttack != null && !tool.surpriseAttack.extraMeleeDamages.NullOrEmpty())
                {
                    extraDamages = extraDamages.Concat(tool.surpriseAttack.extraMeleeDamages);
                }
                foreach (ExtraDamage extraDamage in extraDamages)
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
            Pawn pawn = target.Thing as Pawn;
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            foreach (DamageInfo item in DamageInfosToApply(target))
            {
                if (target.ThingDestroyed)
                {
                    return result;
                }

                if(pawn.RaceProps.IsFlesh)
                    result.AddHediff(pawn.health.AddHediff(tool.hediff, item.HitPart));

                result = target.Thing.TakeDamage(item);
            }
            return result;
        }
    }
}
