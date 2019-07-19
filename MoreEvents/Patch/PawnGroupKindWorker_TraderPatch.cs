using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader))]
    [HarmonyPatch("GenerateTrader")]
    public class PawnGroupKindWorker_TraderPatch
    {
        static void Postfix(Pawn __result)
        {
            if (Rand.Chance(0.15f))
            {
                if (__result != null)
                {
                    QuestsHandler.TryGiveRandomQuestTo(__result);
                }
            }
        }
    }
}
