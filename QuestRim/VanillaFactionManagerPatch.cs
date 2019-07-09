using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{

    [HarmonyPatch(typeof(FactionManager))]
    [HarmonyPatch("Add")]
    public class VanillaFactionManagerPatch_Add
    {
        public static void Postfix(ref Faction faction)
        {
            QuestsManager.Communications.FactionManager.Add(faction);
        }
    }

    [HarmonyPatch(typeof(FactionManager))]
    [HarmonyPatch("Remove")]
    public class VanillaFactionManagerPatch_Remove
    {
        public static void Postfix(ref Faction faction)
        {
            QuestsManager.Communications.FactionManager.Remove(faction);
        }
    }
}
