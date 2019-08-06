using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public static class QuestsHandler
    {
        public static int QuestsCount => DefDatabase<QuestDef>.DefCount;

        public static QuestSite CreateSiteFor(this Quest quest, int tile, Faction faction)
        {
            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = tile;
            questPlace.SetFaction(faction);
            questPlace.Init(quest);
            quest.Site = questPlace;
            quest.Target = questPlace;

            return questPlace;
        }

        public static bool TryGiveRandomQuestsTo(Pawn pawn, int min, int max)
        {
            if (QuestsCount < max)
                return false;

            if (min < 0)
                return false;

            int count = Rand.Range(min, max);

            return TryGiveRandomQuestTo(pawn, count);
        }

        public static bool TryGiveRandomQuestTo(Pawn pawn)
        {
            return TryGiveRandomQuestTo(pawn, 1);
        }

        public static bool TryGiveRandomQuestTo(Pawn pawn, int giveCount)
        {
            try
            {
                if (pawn == null)
                    return false;

                Dictionary<IncidentDef, int> lastFireTicks = Find.World.StoryState.lastFireTicks;

                if (lastFireTicks == null)
                    return false;

                int ticksGame = Find.TickManager.TicksGame;

                List<QuestDef> allQuests = DefDatabase<QuestDef>.AllDefsListForReading;

                if (allQuests.Count < giveCount)
                    return false;

                List<QuestDef> allQuestsToFire = new List<QuestDef>();

                QuestPawn questPawn = null;
                pawn.GetQuestPawn(out questPawn);

                foreach (var potentialQuest in allQuests)
                {
                    if (potentialQuest.Incident == null)
                    {
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

                for (int i = 0; i < giveCount; i++)
                {
                    if (allQuestsToFire.Count == 0)
                        return true;

                    if (allQuestsToFire.TryRandomElementByWeight(w => w.Commonality, out QuestDef result))
                    {
                        TryGiveQuestTo(pawn, result);
                        allQuestsToFire.Remove(result);
                    }
                }
                return false;
            }catch(Exception ex)
            {
                Log.Message($"Error while giving quest --> {ex}");

                return false;
            }
        }

        public static bool TryGiveQuestTo(Pawn pawn, QuestDef questDef)
        {
            Quest quest = (Quest)Activator.CreateInstance(questDef.Quest);
            if(quest.TryGiveQuestTo(pawn, questDef))
            {
                FiringIncident inc = new FiringIncident
                {
                    def = questDef.Incident,
                    parms = new IncidentParms()
                    {
                        forced = false,
                        target = Find.World
                    }
                };
                Find.World.StoryState.Notify_IncidentFired(inc);

                return true;
            }

            return false;
        }
    }
}
