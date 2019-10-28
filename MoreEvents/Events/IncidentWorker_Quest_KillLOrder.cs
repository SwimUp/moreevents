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
    public class IncidentWorker_Quest_KillLOrder : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Quest_KillOrder"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!TryResolveTwoFaction(out Faction faction1, out Faction faction2))
                return false;

            foreach (var quest in QuestsManager.Communications.Quests)
            {
                if (quest.Faction == faction1 && quest is Quest_KillOrder)
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryResolveTwoFaction(out Faction faction1, out Faction faction2))
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 7, 15, out int result))
                return false;

            Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier, faction2);
            Find.WorldPawns.PassToWorld(pawn);

            Quest_KillOrder quest = new Quest_KillOrder(pawn, Rand.Range(10, 17));
            quest.Faction = faction1;
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.GenerateRewards();

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = result;
            questPlace.SetFaction(faction1);
            questPlace.Init(quest);
            questPlace.RemoveAfterLeave = false;

            quest.Target = questPlace;
            quest.Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            string description = string.Format(def.letterText, faction1.Name, pawn.Name.ToString(), faction2.Name);
            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: description, lookTarget: questPlace));

            return true;
        }

        private bool TryResolveTwoFaction(out Faction faction1, out Faction faction2)
        {
            faction2 = null;

            faction1 = Find.FactionManager.RandomNonHostileFaction();
            if (faction1 == null)
                return false;

            Faction faction3 = faction1;
            if(Find.FactionManager.AllFactionsVisible.Where(f => f != Faction.OfPlayer && f.HostileTo(faction3)).TryRandomElement(out faction2))
            {
                return true;
            }

            return false;
        }
    }
}
