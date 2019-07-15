using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{

    [HarmonyPatch(typeof(RimWorld.FactionManager))]
    [HarmonyPatch("Add")]
    public class VanillaFactionManagerPatch_Add
    {
        static void Postfix(ref Faction faction)
        {
            Log.Message("YES");
            QuestsManager.Communications.FactionManager.Add(faction);
        }
    }

    [HarmonyPatch(typeof(RimWorld.FactionManager))]
    [HarmonyPatch("Remove")]
    public class VanillaFactionManagerPatch_Remove
    {
        static void Postfix(ref Faction faction)
        {
            QuestsManager.Communications.FactionManager.Remove(faction);
        }
    }
}
