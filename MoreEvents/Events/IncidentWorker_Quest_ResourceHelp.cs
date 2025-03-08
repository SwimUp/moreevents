﻿using MoreEvents.Quests;
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
    public class IncidentWorker_Quest_ResourceHelp : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_ResourceHelp"];

        private readonly SimpleCurve ValueFactorFromWealthCurve = new SimpleCurve
        {
            new CurvePoint(0, 1),
            new CurvePoint(50000, 1.5f),
            new CurvePoint(300000, 2.2f)
        };

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();
            if (faction == null)
                return false;

            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == faction && quest is Quest_ThingsHelp)
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
            List<Settlement> factionBases = Find.WorldObjects.Settlements.Where(x => x.Faction == faction
            && CaravanArrivalTimeEstimator.EstimatedTicksToArrive(Find.AnyPlayerHomeMap.Tile, x.Tile, null).TicksToDays() < 7).ToList();

            if (factionBases.Count == 0)
                return false;

            Settlement factionBase = factionBases.RandomElement();

            if (Find.WorldObjects.WorldObjectAt(factionBase.Tile, QuestRim.WorldObjectDefOfLocal.QuestPlace) != null)
                return false;

            Quest_ThingsHelp quest = new Quest_ThingsHelp
            {
                id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID(),
                Faction = faction
            };
            float marketValue = GenerateRequestItems(quest, Find.AnyPlayerHomeMap);
            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(marketValue * 1.3f, marketValue * 1.7f), new IntRange(3, 6), null, null);

            LookTargets target = new LookTargets(factionBase.Tile);
            quest.Target = target;
            quest.TicksToPass = Rand.Range(12, 25) * 60000;

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = factionBase.Tile;
            questPlace.SetFaction(faction);
            questPlace.Init(quest);
            quest.Site = questPlace;

            Find.WorldObjects.Add(questPlace);
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: def.description, lookTarget: target));

            return true;
        }

        private float GenerateRequestItems(Quest_ThingsHelp quest, Map map)
        {
            int totalCount = 0;
            int maxCount = (int)(Rand.Range(2f, 7f) * ValueFactorFromWealthCurve.Evaluate(map.wealthWatcher.WealthTotal));
            float marketValue = 0;
            do
            {
                ThingDef thingDef = RandomRequiredDef();
                int num = Rand.Range(20, 150);
                if (num > thingDef.stackLimit)
                {
                    num = thingDef.stackLimit;
                }

                if (quest.RequestItems.ContainsKey(thingDef))
                    quest.RequestItems[thingDef] += num;
                else
                    quest.RequestItems.Add(thingDef, num);

                totalCount++;

                marketValue += num * thingDef.BaseMarketValue;
            }
            while (totalCount < maxCount);

            return marketValue;
        }

        private static IEnumerable<ThingDef> PossibleСontentsDefs()
        {
            return from d in DefDatabase<ThingDef>.AllDefs
                   where d.category == ThingCategory.Item && d.tradeability.TraderCanSell() && d.equipmentType == EquipmentType.None && d.BaseMarketValue >= 1f && d.BaseMarketValue < 40f && !d.HasComp(typeof(CompHatcher))
                   select d;
        }

        private static ThingDef RandomRequiredDef()
        {
            int numMeats = (from x in PossibleСontentsDefs()
                            where x.IsMeat
                            select x).Count();
            int numLeathers = (from x in PossibleСontentsDefs()
                               where x.IsLeather
                               select x).Count();
            return PossibleСontentsDefs().RandomElementByWeight((ThingDef d) => ThingSetMakerUtility.AdjustedBigCategoriesSelectionWeight(d, numMeats, numLeathers));
        }

        private Faction GetFaction()
        {
            return Find.FactionManager.RandomAlliedFaction();
        }
    }
}
