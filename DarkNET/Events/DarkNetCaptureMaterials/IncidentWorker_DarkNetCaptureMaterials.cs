using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Events.DarkNetCaptureMaterials
{
    public class IncidentWorker_DarkNetCaptureMaterials : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            DarkNet darkNet = Current.Game.GetComponent<DarkNet>();
            if (darkNet == null || darkNet.Traders == null)
                return false;

            DarkNetTrader trader = darkNet.Traders.RandomElement();
            Faction attacker = Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Pirate);

            if (attacker == null || attacker.defeated)
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 5, 12, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i)))
                return false;

            Quest_DarkNetCaptureMaterials quest = new Quest_DarkNetCaptureMaterials(trader.def);
  
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.TicksToPass = 7 * 60000;
            quest.Faction = attacker;

            string title = string.Format(def.letterLabel, trader.def.LabelCap);
            string desc = string.Format(def.letterText, trader.def.LabelCap);

            QuestSite site = QuestsHandler.CreateSiteFor(quest, newTile, quest.Faction);

            Find.WorldObjects.Add(site);

            QuestsManager.Communications.AddQuest(quest);

            Find.LetterStack.ReceiveLetter(title, desc, def.letterDef, new LookTargets(site));

            return true;
        }
    }
}
