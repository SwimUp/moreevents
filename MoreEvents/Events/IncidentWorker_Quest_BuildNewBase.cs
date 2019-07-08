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

            if (!GetNewTile(factionBase.Tile, out int newTile))
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

        private Faction GetFaction()
        {
            return Find.FactionManager.RandomAlliedFaction();
        }

        private bool GetNewTile(int root, out int newTile)
        {
            return TileFinder.TryFindPassableTileWithTraversalDistance(root, 5, 9, out newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i));
        }
    }
}
