using Harmony;
using MoreEvents.Things.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(GameCondition_ToxicFallout))]
    [HarmonyPatch("DoPawnsToxicDamage")]
    class ToxicFallout_Patch
    {
        static bool Prefix(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (CanDamage(pawn) && !pawn.Position.Roofed(map) && pawn.def.race.IsFlesh && !Apparel_Mk1.HasMk1Enable(pawn, RimArmorCore.ThingDefOfLocal.Apparel_MK1ThunderHead))
                {
                    float num = 0.028758334f;
                    num *= pawn.GetStatValue(StatDefOf.ToxicSensitivity);
                    if (num != 0f)
                    {
                        float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 0x46EDC5D));
                        num *= num2;
                        HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, num);
                    }
                }
            }
            return false;
        }

        private static bool CanDamage(Pawn p)
        {
            var app = Apparel_MkArmor.HasAnyMK(p);
            if (app != null)
            {
                foreach (var armorSlot in app.Slots)
                {
                    foreach (var slot in armorSlot.Modules)
                    {
                        if (slot.Module != null)
                        {
                            if (!slot.Module.CanAffectCondition(GameConditionDefOfLocal.IceStorm))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
