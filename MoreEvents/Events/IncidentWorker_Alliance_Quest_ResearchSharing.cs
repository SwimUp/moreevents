using QuestRim;
using RimOverhaul.Quests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events
{
    public class IncidentWorker_Alliance_Quest_ResearchSharing : IncidentWorker
    {
        private SkillDef SkillDef => SkillDefOf.Intellectual;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (Find.ResearchManager.currentProj == null)
                return false;

            Alliance alliance = parms.target as Alliance;
            if (alliance == null)
                return false;

            return alliance.PlayerOwner;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Alliance alliance = parms.target as Alliance;
            if (alliance.Factions.Count == 0)
                return false;

            FactionInteraction factionInteraction = alliance.Factions.RandomElement();

            if (!Find.WorldObjects.Settlements.Where(x => x.Faction == factionInteraction.Faction
                && CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Find.AnyPlayerHomeMap.Tile, x.Tile, null).TicksToDays() < 7).TryRandomElement(out Settlement settlement))
                return false;

            var research = Find.ResearchManager.currentProj;
            int pointsReward = (int)((research.baseCost - Find.ResearchManager.GetProgress(research)) * 0.3f);
            Alliance_Quest_ResearchSharing quest = new Alliance_Quest_ResearchSharing(factionInteraction.Faction, SkillDef, pointsReward);
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.TicksToPass = 6 * 60000;
            quest.IncidentDef = def;
            QuestSite questSite = quest.CreateSiteFor(settlement.Tile, quest.Faction);

            Find.WorldObjects.Add(questSite);

            QuestsManager.Communications.AddQuest(quest, null);

            SendStandardLetter(parms, questSite, factionInteraction.Faction.Name, pointsReward.ToString());

            return true;
        }
    }
}
