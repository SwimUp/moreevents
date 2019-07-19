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
    [HarmonyPatch("GenerateGuards")]
    public class PawnGroupKindWorker_TraderPatch
    {
        static void Postfix(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, Pawn trader,
            List<Thing> wares, List<Pawn> outPawns)
        {
            if (outPawns.Count > 0)
            {
                if (outPawns.Where(p => !p.NonHumanlikeOrWildMan() && p.Faction != null).TryRandomElement(out Pawn result))
                {
                    QuestsHandler.TryGiveRandomQuestTo(result);
                }
            }
        }
    }
}
