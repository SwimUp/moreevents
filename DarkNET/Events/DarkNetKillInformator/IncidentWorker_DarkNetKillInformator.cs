using QuestRim;
using RimOverhaul.Gss;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Events.DarkNetKillInformator
{
    public class IncidentWorker_DarkNetKillInformator : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 7, 15, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i)))
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 7, 15, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i)))
                return false;

            if (GssRaids.GssFaction == null)
                return false;

            DarkNet darkNet = Current.Game.GetComponent<DarkNet>();
            if (darkNet == null || darkNet.Traders == null)
                return false;

            DarkNetTrader trader = darkNet.Traders.RandomElement();

            Quest_DarkNetKillInformator quest = new Quest_DarkNetKillInformator(trader.def);
            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(1200, 2500), new IntRange(1, 4), null, null);

            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.TicksToPass = 10 * 60000;
            quest.Faction = GssRaids.GssFaction;

            string title = string.Format(def.letterLabel, trader.def.LabelCap, quest.Faction.Name);
            string desc = string.Format(def.letterText, trader.def.LabelCap, quest.Faction.Name);

            QuestSite site = QuestsHandler.CreateSiteFor(quest, newTile, quest.Faction);

            Find.WorldObjects.Add(site);

            QuestsManager.Communications.AddQuest(quest);

            Find.LetterStack.ReceiveLetter(title, desc, def.letterDef, new LookTargets(site));

            return true;
        }
    }
}
