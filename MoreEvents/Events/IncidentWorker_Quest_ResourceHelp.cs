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
    public class IncidentWorker_Quest_ResourceHelp : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_ResourceHelp"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (GetFaction() == null)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();
            List<Settlement> factionBases = Find.WorldObjects.Settlements.Where(x => x.Faction == faction).ToList();

            if (factionBases.Count == 0)
                return false;

            Settlement factionBase = factionBases.RandomElement();

            if (Find.WorldObjects.WorldObjectAt(factionBase.Tile, QuestRim.WorldObjectDefOfLocal.QuestPlace) != null)
                return false;

            Quest_ThingsHelp quest = new Quest_ThingsHelp();
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.Faction = faction;
            float marketValue = GenerateRequestItems(quest);

            ThingSetMakerParams parms2 = default;
            parms2.totalMarketValueRange = new FloatRange(marketValue * 0.8f, marketValue * 1.3f);
            quest.Rewards = ThingSetMakerDefOf.ResourcePod.root.Generate(parms2);

            LookTargets target = new LookTargets(factionBase.Tile);
            quest.Target = target;
            quest.TicksToPass = Rand.Range(5, 11) * 60000;

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = factionBase.Tile;
            questPlace.SetFaction(faction);
            questPlace.Init(quest);

            Find.WorldObjects.Add(questPlace);
            QuestsManager.Communications.AddQuest(quest);

            SendStandardLetter(target);

            return true;
        }

        private float GenerateRequestItems(Quest_ThingsHelp quest)
        {
            int totalCount = 0;
            int maxCount = Rand.Range(2, 7);
            float marketValue = 0;
            do
            {
                ThingDef thingDef = RandomRequiredDef();
                int num = Rand.Range(20, 40);
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
