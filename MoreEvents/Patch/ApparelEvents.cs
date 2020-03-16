using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker))]
    [HarmonyPatch("Notify_ApparelAdded")]
    public class ApparelEvents_Notify_ApparelAdded
    {
        public static void Postfix(Pawn_ApparelTracker __instance, ref Apparel apparel)
        {
            if(apparel.AllComps != null)
            {
                foreach(var comp in apparel.AllComps)
                {
                    comp.Notify_SignalReceived(new Signal("apparel-wear", __instance.pawn));
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker))]
    [HarmonyPatch("Notify_ApparelRemoved")]
    public class ApparelEvents_Notify_ApparelRemoved
    {
        public static void Postfix(Pawn_ApparelTracker __instance, ref Apparel apparel)
        {
            if (apparel.AllComps != null)
            {
                foreach (var comp in apparel.AllComps)
                {
                    comp.Notify_SignalReceived(new Signal("apparel-unwear", __instance.pawn));
                }
            }
        }
    }
}
