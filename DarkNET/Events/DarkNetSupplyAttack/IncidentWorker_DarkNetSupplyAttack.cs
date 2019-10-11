using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET.Events.DarkNetSupplyAttack
{
    public class IncidentWorker_DarkNetSupplyAttack : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            DarkNet darkNet = Current.Game.GetComponent<DarkNet>();
            if(darkNet != null)
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
            Faction attacker = Find.FactionManager.RandomEnemyFaction();

            if (attacker == null)
                return false;

            if(!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 5, 12, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i)))
                return false;

            if (!trader.TryGetGoods(allTraderThings))
                return false;

            Quest_DarkNetSupplyAttack quest = new Quest_DarkNetSupplyAttack(trader.def);
            int takeCount = Rand.Range(1, (int)Mathf.Max(1, allTraderThings.Count * 0.4f));
            IEnumerable<Thing> rewards = allTraderThings.TakeRandom(takeCount);
            quest.Rewards = new List<Thing>();
            foreach(var reward in rewards)
            {
                Thing copy = ThingMaker.MakeThing(reward.def, reward.Stuff);
                copy.stackCount = (int)Mathf.Max(1, reward.stackCount * 0.6f);

                quest.Rewards.Add(copy);
            }

            if (quest.Rewards.Count == 0)
                return false;

            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.TicksToPass = 6 * 60000;
            quest.ShowInConsole = false; //release = false
            quest.Faction = attacker;

            string title = "Quest_DarkNetSupplyAttack_CardLabel".Translate(trader.def.LabelCap);
            string desc = "Quest_DarkNetSupplyAttack_Description".Translate(trader.def.LabelCap);

            CommunicationDialog dialog = QuestsManager.Communications.AddCommunication(QuestsManager.Communications.UniqueIdManager.GetNextDialogID(), title, desc, incident: def);
            dialog.KnownFaction = false;
            quest.CommunicationDialog = dialog;

            QuestSite site = QuestsHandler.CreateSiteFor(quest, newTile, quest.Faction);

            Find.LetterStack.ReceiveLetter(title, desc, def.letterDef, new LookTargets(site));

            Find.WorldObjects.Add(site);

            QuestsManager.Communications.AddQuest(quest);

            return true;
        }
    }
}
