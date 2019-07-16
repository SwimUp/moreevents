using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public static class Extensions
    {
        public static bool Contains(this List<FactionInteraction> list, Faction faction)
        {
            foreach(var inter in list)
            {
                if(inter.Faction == faction)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetQuestPawn(this Pawn pawn, out QuestPawn questPawn)
        {
            questPawn = QuestsManager.Communications.QuestPawns.FirstOrDefault(p => p.Pawn == pawn);
            return questPawn != null;
        }

        public static bool CanGetQuests(this Pawn pawn)
        {
            if (pawn.Dead || !pawn.Spawned || !pawn.CanCasuallyInteractNow() ||
                (pawn.Downed) ||
                (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer))) return false;

            return pawn.GetQuestPawn(out QuestPawn questPawn);
        }
    }
}
