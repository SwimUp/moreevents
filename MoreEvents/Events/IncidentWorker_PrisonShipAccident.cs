using MoreEvents;
using QuestRim;
using RimOverhaul.Gss;
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
    public class IncidentWorker_PrisonShipAccident : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["PrisonShipAccident"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 3, 10, out int result))
                return false;

            Faction gss = GssRaids.GssFaction;
            if (gss == null)
                return false;

            Quest_PrisonShipAccident quest = new Quest_PrisonShipAccident();
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.Faction = gss;
            quest.TicksToPass = Rand.Range(3, 5) * 60000;
            quest.GenerateRewards();
            quest.IncidentDef = def;
            QuestSite questSite = quest.CreateSiteFor(result, quest.Faction);

            Find.WorldObjects.Add(questSite);

            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: def.letterText, lookTarget: questSite));

            return true;
        }
    }
}
