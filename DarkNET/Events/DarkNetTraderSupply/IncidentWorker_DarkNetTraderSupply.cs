using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Events.DarkNetTraderSupply
{
    public class IncidentWorker_DarkNetTraderSupply : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            DarkNet darkNet = Current.Game.GetComponent<DarkNet>();
            if (darkNet != null)
            {
                if (darkNet.Traders != null && !darkNet.Traders.Any(x => x.Online))
                    return false;
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            DarkNet darkNet = Current.Game.GetComponent<DarkNet>();
            if (darkNet == null || darkNet.Traders == null)
                return false;

            DarkNetTrader trader = darkNet.Traders.RandomElement();
            List<Thing> allTraderThings = new List<Thing>();
            Faction buyer = Find.FactionManager.RandomEnemyFaction();
            Faction garant = null;
            if (!(from x in Find.FactionManager.AllFactions
                 where !x.IsPlayer && !x.def.hidden && !x.defeated && x.def.humanlikeFaction && x.HostileTo(Faction.OfPlayer) && x != buyer
                  select x).TryRandomElement(out garant))
            {
                return false;
            }

            if (buyer == null)
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 3, 6, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i)))
                return false;

            if (!trader.TryGetGoods(allTraderThings))
                return false;

            Quest_DarkNetTraderSupply quest = new Quest_DarkNetTraderSupply(trader.def, garant);
            int takeCount = Rand.Range(1, (int)Mathf.Max(1, allTraderThings.Count * 0.4f));
            IEnumerable<Thing> rewards = allTraderThings.Where(x => !(x is MinifiedThing)).ToList().TakeRandom(takeCount);
            quest.Rewards = new List<Thing>();
            foreach (var reward in rewards)
            {
                Thing copy = ThingMaker.MakeThing(reward.def, reward.Stuff);
                copy.stackCount = (int)Mathf.Max(1, reward.stackCount * 0.6f);

                quest.Rewards.Add(copy);
            }

            if (quest.Rewards.Count == 0)
                return false;

            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.TicksToPass = 4 * 60000;
            quest.Faction = buyer;
            quest.ShowInConsole = false;

            string title = string.Format(def.letterLabel, trader.def.LabelCap);
            string desc = string.Format(def.letterText, trader.def.LabelCap, buyer.Name, garant.Name);

            CommunicationDialog dialog = QuestsManager.Communications.AddCommunication(QuestsManager.Communications.UniqueIdManager.GetNextDialogID(), title, desc, incident: def);
            dialog.KnownFaction = true;
            quest.CommunicationDialog = dialog;

            QuestSite site = QuestsHandler.CreateSiteFor(quest, newTile, quest.Faction);

            Find.WorldObjects.Add(site);

            QuestsManager.Communications.AddQuest(quest);

            Find.LetterStack.ReceiveLetter(title, desc, def.letterDef, new LookTargets(site));

            return true;
        }
    }
}
