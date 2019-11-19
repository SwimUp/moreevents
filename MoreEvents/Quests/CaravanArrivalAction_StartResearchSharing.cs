using MoreEvents.Quests;
using QuestRim;
using RimOverhaul.Quests;
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
    public class CaravanArrivalAction_StartResearchSharing : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Alliance_Quest_ResearchSharing Quest => quest;
        private Alliance_Quest_ResearchSharing quest;

        public override string Label => "Quest_MissingPeople_CardLabel".Translate();

        public override string ReportString => "Quest_MissingPeople_CardLabel".Translate();

        public CaravanArrivalAction_StartResearchSharing()
        {
        }

        public CaravanArrivalAction_StartResearchSharing(MapParent mapParent, Alliance_Quest_ResearchSharing quest)
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
            Pawn p = caravan.PawnsListForReading.First();
            Quest.StartResearchSharing(p);

            caravan.pawns.Remove(p);

            if (caravan.pawns.Count == 0)
                Find.WorldObjects.Remove(caravan);
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }

            return true;
        }

        public FloatMenuAcceptanceReport CanUse(Caravan caravan)
        {
            if(Quest.Started)
            {
                return FloatMenuAcceptanceReport.WithFailReason("CaravanArrivalAction_StartResearchSharing_Already".Translate());
            }

            return true;
        }
    }
}
