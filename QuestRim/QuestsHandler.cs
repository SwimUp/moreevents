using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public static class QuestsHandler
    {
        public static bool TryGiveRandomQuestTo(Pawn pawn)
        {
            Dictionary<IncidentDef, int> lastFireTicks = Find.World.StoryState.lastFireTicks;
            int ticksGame = Find.TickManager.TicksGame;

            List<QuestDef> allQuests = DefDatabase<QuestDef>.AllDefsListForReading;
            List<QuestDef> allQuestsToFire = new List<QuestDef>();

            QuestPawn questPawn = null;
            pawn.GetQuestPawn(out questPawn);

            foreach (var potentialQuest in allQuests)
            {
                if (potentialQuest.Incident == null)
                {
                    Log.Warning($"{potentialQuest.defName} has null incident");
                    continue;
                }

                if (lastFireTicks.TryGetValue(potentialQuest.Incident, out int value))
                {
                    float num = (float)(ticksGame - value) / 60000f;
                    if (num < potentialQuest.Incident.minRefireDays)
                        continue;
                }

                if (QuestsManager.Communications.Quests.Any(q => q.RelatedQuestDef == potentialQuest && q.Faction == pawn.Faction))
                    continue;

                if (questPawn != null)
                {
                    if (questPawn.Quests.Any(q => q.RelatedQuestDef == potentialQuest))
                        continue;
                }

                allQuestsToFire.Add(potentialQuest);
            }

            if (allQuestsToFire.Count == 0)
                return false;

            if(allQuestsToFire.TryRandomElementByWeight(w => w.Commonality, out QuestDef result))
            {
                FiringIncident inc = new FiringIncident
                {
                    def = result.Incident,
                    parms = new IncidentParms()
                    {
                        forced = false,
                        target = Find.World
                    }
                };
                Find.World.StoryState.Notify_IncidentFired(inc);

                Quest quest = (Quest)Activator.CreateInstance(result.Quest);
                return quest.TryGiveQuestTo(pawn, result);
            }

            return false;
        }
    }
}
