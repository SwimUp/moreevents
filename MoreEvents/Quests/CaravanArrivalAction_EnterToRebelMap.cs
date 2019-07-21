using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Quests
{
    public class CaravanArrivalAction_EnterToRebelMap : CaravanArrivalAction_EnterToQuestMap
    {
        public override IntVec3 MapSize => new IntVec3(170, 1, 170);

        public Quest_SuppressionRebellion Quest => quest;
        private Quest_SuppressionRebellion quest;

        public override string Label => "Quest_SuppressionRebellion_CardLabel".Translate();

        public override string ReportString => "Quest_SuppressionRebellion_CardLabel".Translate();

        public CaravanArrivalAction_EnterToRebelMap()
        {
        }

        public CaravanArrivalAction_EnterToRebelMap(MapParent mapParent, Quest_SuppressionRebellion quest) : base(mapParent)
        {
            this.quest = quest;
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (Quest.AttackFail)
            {
                return FloatMenuAcceptanceReport.WithFailReason("CaravanArrivalAction_EnterToRebelMap_AlreadyStart".Translate());
            }

            return base.StillValid(caravan, destinationTile);
        }

        public override FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (Quest.AttackFail)
            {
                return FloatMenuAcceptanceReport.WithFailReason("CaravanArrivalAction_EnterToRebelMap_AlreadyStart".Translate());
            }

            return base.CanVisit(caravan, mapParent);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref quest, "Quest");
        }
    }
}
