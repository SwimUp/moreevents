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
    public class IncidentWorker_Quest_SuppressionRebellion : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_SuppressionRebellion"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();
            if (faction == null)
                return false;

            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == faction && quest is Quest_SuppressionRebellion)
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Faction faction = GetFaction();
            if (!Find.WorldObjects.Settlements.Where(x => x.Faction == faction
            && CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Find.AnyPlayerHomeMap.Tile, x.Tile, null).TicksToDays() < 6).TryRandomElement(out Settlement factionBase))
                return false;

            List<int> neighbors = new List<int>();
            Find.WorldGrid.GetTileNeighbors(factionBase.Tile, neighbors);

            if (neighbors.Count == 0)
                return false;

            int rebelPos = neighbors.RandomElement();

            Quest_SuppressionRebellion quest = new Quest_SuppressionRebellion();
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.Faction = factionBase.Faction;
            quest.TicksToPass = Rand.Range(10, 20) * 60000;
            quest.GenerateRewards(quest.GetQuestThingFilter(), quest.Faction.GetRangeByFactionTechLevel(400, 700), new IntRange(3, 8), null, null);
            QuestSite questSite = quest.CreateSiteFor(rebelPos, quest.Faction);

            quest.RebelSettlement = factionBase;
            questSite.RemoveAfterLeave = false;

            Find.WorldObjects.Add(questSite);

            string desc = string.Format(def.letterText, factionBase.Faction.leader.Name.ToStringFull, factionBase.Name, factionBase.Faction.Name);
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: desc, lookTarget: questSite));

            return true;
        }

        private Faction GetFaction()
        {
            return Find.FactionManager.RandomNonHostileFaction();
        }
    }
}
