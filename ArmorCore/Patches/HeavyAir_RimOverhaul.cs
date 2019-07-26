using Harmony;
using MoreEvents.Events;
using MoreEvents.Things.Mk1;
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
            if(Apparel_Mk1.HasMk1Enable(pawn))
            {
                __result = false;
                return;
            }

            __result =  true;
        }
    }
}
