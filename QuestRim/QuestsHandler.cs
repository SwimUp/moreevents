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

            Log.Message($"Quest count (total): {QuestsCount}, trying find min: {min} and {max}");

            if (min < 0)
                return false;

            int count = Rand.Range(min, max);

            Log.Message($"Quest count (total): {QuestsCount}, quest count {count}");

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
                {
                    Log.Error($"Error: pawn is null");
                    return false;
                }

                Dictionary<IncidentDef, int> lastFireTicks = Find.World.StoryState.lastFireTicks;

                if (lastFireTicks == null)
                {
                    Log.Warning($"Error: lastFireTicks world is null");
                    return false;
                }

                int ticksGame = Find.TickManager.TicksGame;

                Log.Message($"Ticks game: {ticksGame}");

                List<QuestDef> allQuests = DefDatabase<QuestDef>.AllDefsListForReading;

                Log.Message($"All QuestDef {allQuests}, giveCount: {giveCount}");

                if (allQuests.Count < giveCount)
                {
                    Log.Warning($"All QuestDef < giveCount, exit");
                    return false;
                }

                List<QuestDef> allQuestsToFire = new List<QuestDef>();

                QuestPawn questPawn = null;
                pawn.GetQuestPawn(out questPawn);

                foreach (var potentialQuest in allQuests)
                {
                    Log.Message($"Check --> {potentialQuest.defName}");

                    if (potentialQuest.Incident == null)
                    {
                        continue;
                    }

                    if (lastFireTicks.TryGetValue(potentialQuest.Incident, out int value))
                    {
                        float num = (float)(ticksGame - value) / 60000f;
                        Log.Message($"Last fire days: {num}");
                        if (num < potentialQuest.Incident.minRefireDays)
                        {
                            Log.Message($"The minimum number of days has not passed {potentialQuest.Incident.minRefireDays}, skip");
                            continue;
                        }
                    }

                    if (QuestsManager.Communications.Quests.Any(q => q.RelatedQuestDef == potentialQuest && q.Faction == pawn.Faction))
                    {
                        Log.Message($"Same quest from same faction already active, skip");
                        continue;
                    }

                    if (questPawn != null)
                    {
                        if (questPawn.Quests.Any(q => q.RelatedQuestDef == potentialQuest))
                        {
                            Log.Message($"This questpawn already have this quest, skip");
                            continue;
                        }
                    }

                    allQuestsToFire.Add(potentialQuest);
                    Log.Message($"Added to random list");
                }

                if (allQuestsToFire.Count == 0)
                {
                    Log.Message($"No quests to choose, end");
                    return false;
                }

                for (int i = 0; i < giveCount; i++)
                {
                    if (allQuestsToFire.Count == 0)
                    {
                        Log.Message($"No quests to choose, end in count {i}");
                        return true;
                    }

                    if (allQuestsToFire.TryRandomElementByWeight(w => w.Commonality, out QuestDef result))
                    {
                        Log.Message($"Selected quest: {result.defName}");
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
            Log.Message($"Trying give {questDef.defName} to {pawn}");
            Quest quest = (Quest)Activator.CreateInstance(questDef.Quest);
            if(quest.TryGiveQuestTo(pawn, questDef))
            {
                Log.Message($"Conditions ok");

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

                Log.Message($"end");

                return true;
            }

            Log.Message($"Conditions for the selected quests are not met");

            return false;
        }
    }
}
