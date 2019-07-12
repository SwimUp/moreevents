using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_Quest_BuildNewBase : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_BuildNewBase"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();
            if (faction == null)
                return false;

            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == faction && quest is Quest_BuildNewBase)
                {
                    return false;
                }
            }

            return true;
        }


        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();

            foreach (var q in QuestsManager.Communications.Quests)
            {
                if (q.Faction == faction && q is Quest_BuildNewBase)
                {
                    return false;
                }
            }

            List<Settlement> factionBases = Find.WorldObjects.Settlements.Where(x => x.Faction == faction).ToList();

            if (factionBases.Count == 0)
                return false;

            Settlement factionBase = factionBases.RandomElement();

            if (!TryGetNewTile(factionBase.Tile, out int newTile))
                return false;

            Quest_BuildNewBase quest = new Quest_BuildNewBase();
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.Faction = faction;

            int playerPawns = 0;
            foreach(var map in Find.Maps)
            {
                if(map.IsPlayerHome)
                {
                    playerPawns += map.mapPawns.ColonistCount;
                }
            }
            playerPawns = Mathf.Max(2, playerPawns);
            quest.PawnsRequired = Rand.Range(1, playerPawns);

            float value = (250 * quest.PawnsRequired) * 1.8f;

            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(value, value), new IntRange(2, 5), null, null);

            LookTargets target = new LookTargets(newTile);
            quest.Target = target;
            quest.TicksToPass = Rand.Range(8, 14) * 60000;
            quest.TicksToEnd = Rand.Range(3, 5) * 60000;

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            quest.Site = questPlace;
            quest.OldSettlement = factionBase;

            questPlace.Tile = newTile;
            questPlace.SetFaction(faction);
            questPlace.Init(quest);

            Find.WorldObjects.Add(questPlace);

            string text = string.Format(def.letterText, faction.Name).CapitalizeFirst();
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: text, lookTarget: target));

            return true;
        }

        private Faction GetFaction()
        {
            return Find.FactionManager.RandomAlliedFaction();
        }

        private bool TryGetNewTile(int root, out int newTile)
        {
            return TileFinder.TryFindPassableTileWithTraversalDistance(root, 5, 9, out newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i));
        }
    }
}
