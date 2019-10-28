using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_Quest_KillLeader : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_KillLeader"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryResolveTwoFactions(out Faction alliedFaction, out Faction enemyFaction))
                return false;

            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == alliedFaction && quest is Quest_KillLeader)
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryResolveTwoFactions(out Faction alliedFaction, out Faction enemyFaction))
                return false;

            if (enemyFaction.leader == null)
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 7, 16, out int result))
                return false;

            Quest_KillLeader quest = new Quest_KillLeader(enemyFaction.leader, Rand.Range(11, 17));
            quest.Faction = alliedFaction;
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(600, 800) * (float)enemyFaction.def.techLevel, new IntRange(1, 3), null, null);

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = result;
            questPlace.SetFaction(alliedFaction);
            questPlace.Init(quest);
            questPlace.RemoveAfterLeave = false;

            quest.Target = questPlace;
            quest.Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            string description = string.Format(def.letterText, alliedFaction.Name, enemyFaction.leader.Name.ToString(), enemyFaction.Name);
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: description, lookTarget: questPlace));

            return true;
        }

        private bool TryResolveTwoFactions(out Faction alliedFaction, out Faction enemyFaction)
        {
            enemyFaction = null;
            alliedFaction = null;

            Faction faction1 = Find.FactionManager.RandomNonHostileFaction();
            if (faction1 == null)
                return false;

            if(Find.FactionManager.AllFactionsVisible.Where(f => f != faction1 && f.HostileTo(faction1) && f.HostileTo(Faction.OfPlayer)).TryRandomElement(out enemyFaction))
            {
                alliedFaction = faction1;
                return true;
            }

            return false;
        }
    }
}
