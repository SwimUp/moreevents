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
    public class IncidentWorker_Quest_MissingPeople : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_MissingPeople"];


        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFaction();
            if (faction == null)
                return false;


            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == faction && quest is Quest_MissingPeople)
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
            Map map = Find.AnyPlayerHomeMap;

            if (!TryGetNewTile(map.Tile, out int newTile))
                return false;

            int days = Rand.Range(5, 20);
            int passedDays = Rand.Range(days + 3, days + 7);
            Quest_MissingPeople quest = new Quest_MissingPeople(Rand.Range(3, 9), days, passedDays);
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.Faction = faction;

            int additionalValue = passedDays * 15;
            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(700 + additionalValue, 1400 + additionalValue), new IntRange(3, 8), null, null);

            LookTargets target = new LookTargets(newTile);
            quest.Target = target;

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = newTile;
            questPlace.SetFaction(faction);
            questPlace.Init(quest);
            questPlace.RemoveAfterLeave = false;

            quest.Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            string text = string.Format(def.letterText, faction.Name, days, passedDays, quest.TicksToPass.TicksToDays().ToString("f2")).CapitalizeFirst();
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: text, lookTarget: target));

            return true;
        }

        private Faction GetFaction()
        {
            if ((from x in Find.FactionManager.AllFactions
                 where !x.IsPlayer && !x.def.hidden && !x.defeated && x.def.humanlikeFaction && (x.PlayerRelationKind == FactionRelationKind.Ally || x.PlayerRelationKind == FactionRelationKind.Neutral)
                 select x).TryRandomElement(out Faction result))
            {
                return result;
            }
            return null;
        }

        public static bool TryGetNewTile(int root, out int newTile)
        {
            return TileFinder.TryFindPassableTileWithTraversalDistance(root, 5, 9, out newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i));
        }
    }
}
