using DarkNET.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Dialogs
{
    public class EmailMessageOption_TakeQuest_Laboratory : EmailMessageOption
    {
        public override string Label => "EmailMessageOption_TakeQuest_Laboratory".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 8, 20, out int result))
            {
                Log.Warning("Cannon find place, try again");
                return;
            }

            Faction traderFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.DarkNetTraders);

            Quest_Laboratory quest = new Quest_Laboratory();
            quest.TicksToPass = 10 * 60000;
            quest.Faction = traderFaction;
            quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(2000, 3500), new IntRange(5, 10), null, null);
            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = Rand.Range(1000, 2000);
            quest.Rewards.Add(silver);

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = result;
            questPlace.SetFaction(traderFaction);
            questPlace.Init(quest);
            questPlace.RemoveAfterLeave = false;

            quest.Target = questPlace;
            quest.Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            QuestsManager.Communications.AddQuest(quest, QuestsManager.Communications.MakeQuestLetter(quest, description: quest.Description, lookTarget: questPlace));

            var component = QuestsManager.Communications.Components.Where(x => x is EmailMessageTimeComp_QuestLaboratory).FirstOrDefault();
            if(component!= null)
            {
                QuestsManager.Communications.RemoveComponent(component);
            }
        }
    }
}
