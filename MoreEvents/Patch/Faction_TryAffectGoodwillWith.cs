using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch("TryAffectGoodwillWith")]
    public class Faction_TryAffectGoodwillWith
    {
        public static bool Prefix(Faction __instance, Faction other, int goodwillChange)
        {
            FactionInteraction first = QuestsManager.Communications.FactionManager.GetInteraction(__instance);
            FactionInteraction second = QuestsManager.Communications.FactionManager.GetInteraction(other);

            var war = first.FirstWarWith(second);

            return war == null;
        }
    }
}
