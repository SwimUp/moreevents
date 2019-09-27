using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage))]
    [HarmonyPatch("ApplyMeleeDamageToTarget")]
    public class Verb_MeleeAttack
    {
        public static string AttackSignalTag => "meleeattack";

        public static void Postfix(ref LocalTargetInfo target, Verb_MeleeAttackDamage __instance)
        {
            try
            {
                if (__instance.EquipmentSource != null)
                {
                    ThingWithComps thing = __instance.EquipmentSource;

                    if (thing.AllComps != null)
                    {
                        foreach (var comp in thing.AllComps)
                        {
                            comp.Notify_SignalReceived(new Signal(AttackSignalTag, new object[] { target.Thing, __instance.CasterPawn }));
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
