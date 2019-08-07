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

namespace MoreEvents.Quests
{
    public class CaravanArrivalAction_TransferMissingPeople : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Quest_MissingPeople Quest => quest;
        private Quest_MissingPeople quest;

        public override string Label => "Quest_MissingPeople_CardLabel".Translate();

        public override string ReportString => "Quest_MissingPeople_CardLabel".Translate();

        public CaravanArrivalAction_TransferMissingPeople()
        {
        }

        public CaravanArrivalAction_TransferMissingPeople(MapParent mapParent, Quest_MissingPeople quest)
        {
            this.mapParent = mapParent;
            this.quest = quest;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref mapParent, "Parent");
            Scribe_References.Look(ref quest, "Quest");
        }

        public override void Arrived(Caravan caravan)
        {
            TryTransfer(caravan);
        }

        private void TryTransfer(Caravan caravan)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Quest_MissingPeople_End".Translate());
            int saved = 0;
            foreach (var pawn in quest.SavedPawns)
            {
                if (caravan.ContainsPawn(pawn))
                {
                    caravan.RemovePawn(pawn);
                    saved++;
                    builder.AppendLine($"- {pawn.Name.ToStringFull}");
                    pawn.Destroy();
                }
            }

            EndCondition condition = EndCondition.Success;
            if(saved == quest.SavedPawns.Count)
            {
                builder.Append("Quest_MissingPeople_EndFull".Translate(Quest.Faction.Name));
            }
            else
            {
                if (saved == 0)
                {
                    builder.Append("Quest_MissingPeople_Fiasko".Translate(Quest.Faction.Name));
                    condition = EndCondition.Fail;
                }
                else
                {
                    builder.Append("Quest_MissingPeople_EndPart".Translate());
                }
            }

            if(caravan.pawns.Count == 0)
            {
                Find.WorldObjects.Remove(caravan);
            }

            Find.LetterStack.ReceiveLetter("Quest_MissingPeople_EndQuest".Translate(), builder.ToString(), LetterDefOf.NeutralEvent);
            QuestSite site = (QuestSite)mapParent;
            site.EndQuest(caravan, condition);
        }


        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }

            return true;
        }
    }
}
