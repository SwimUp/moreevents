using Harmony;
using MoreEvents.Events;
using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Patches
{
    [HarmonyPatch(typeof(GameCondition_RadiationFon))]
    [HarmonyPatch("CanDamage")]
    public class RadiationFon_RimOverhaul
    {
        static void Postfix(ref bool __result, Pawn pawn, Map map)
        {
            if (Apparel_Mk1.HasMk1Enable(pawn))
            {
                __result = false;
                return;
            }

            __result = true;
        }
    }
}
