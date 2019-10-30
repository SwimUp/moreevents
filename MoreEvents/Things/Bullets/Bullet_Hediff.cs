using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_Hediff : Bullet
    {
        public virtual HediffDef HediffDef { get; }

        public virtual float SeverityPerShot { get; }

        public virtual bool FleshOnly => false;

        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(base.launcher, hitThing, intendedTarget.Thing, base.equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = def.projectile.damageDef;
                float amount = base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                Vector3 eulerAngles = ExactRotation.eulerAngles;
                float y = eulerAngles.y;
                Thing launcher = base.launcher;
                ThingDef equipmentDef = base.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                var damageWorker = hitThing.TakeDamage(dinfo);
                damageWorker.AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }

                if (FleshOnly && pawn.RaceProps.IsMechanoid)
                    return;

                var bodyPart = damageWorker.LastHitPart;

                if (bodyPart != null)
                {
                    if (pawn.RaceProps.IsFlesh && pawn.health.hediffSet.PartIsMissing(bodyPart))
                        return;

                    Hediff firstHediffOfDef = GetFirstHediffOfDef(HediffDef, pawn, bodyPart);
                    if (firstHediffOfDef != null)
                    {
                        firstHediffOfDef.Severity += SeverityPerShot;
                    }
                    else if (SeverityPerShot > 0f)
                    {
                        firstHediffOfDef = HediffMaker.MakeHediff(HediffDef, pawn);
                        firstHediffOfDef.Severity = SeverityPerShot;
                        pawn.health.AddHediff(firstHediffOfDef, bodyPart);
                    }
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map));
                MoteMaker.MakeStaticMote(ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(ExactPosition, map, Mathf.Sqrt(base.DamageAmount) * 1f, 4f);
                }
            }
        }

        private Hediff GetFirstHediffOfDef(HediffDef def, Pawn pawn, BodyPartRecord part, bool mustBeVisible = false)
        {
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs[i];

                if (hediff.def == def && (!mustBeVisible || hediff.Visible) && hediff.Part == part)
                {
                    return hediff;
                }
            }
            return null;
        }
    }
}
