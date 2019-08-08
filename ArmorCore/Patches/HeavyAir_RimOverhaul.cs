using Harmony;
using MoreEvents;
using MoreEvents.Events;
using MoreEvents.Things.Mk1;
using RimArmorCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ArmorCore.Patches
{
    [HarmonyPatch(typeof(GameCondition_HeavyAir))]
    [HarmonyPatch("CanDamage")]
    public class HeavyAir_RimOverhaul
    {
        static void Postfix(ref bool __result, Pawn pawn, Map map)
        {
            var app = Apparel_MkArmor.HasAnyMK(pawn);
            if(app != null)
            {
                foreach(var armorSlot in app.Slots)
                {
                    foreach(var slot in armorSlot.Modules)
                    {
                        if(slot.Module != null)
                        {
                            if(!slot.Module.CanAffectCondition(GameConditionDefOfLocal.HeavyAir))
                            {
                                __result = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
