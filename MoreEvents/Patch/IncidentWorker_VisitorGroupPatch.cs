using HarmonyLib;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup))]
    [HarmonyPatch("TryConvertOnePawnToSmallTrader")]
    public class IncidentWorker_VisitorGroupPatch
    {
        static void Postfix(List<Pawn> pawns, Faction faction, Map map)
        {
            if (pawns.Count == 0)
                return;

            Pawn pawn = pawns.Where(p => !p.NonHumanlikeOrWildMan()).RandomElement();
            if (pawn != null && pawn.Faction != null)
            {
                QuestsHandler.TryGiveRandomQuestTo(pawn);
            }
        }
    }
}
