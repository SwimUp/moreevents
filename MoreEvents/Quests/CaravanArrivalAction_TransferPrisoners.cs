using MoreEvents.Quests;
using QuestRim;
using RimOverhaul.Gss;
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
    public class CaravanArrivalAction_TransferPrisoners : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Quest_PrisonShipAccident Quest => quest;
        private Quest_PrisonShipAccident quest;

        public override string Label => Quest.CardLabel;

        public override string ReportString => Quest.CardLabel;

        public CaravanArrivalAction_TransferPrisoners()
        {
        }

        public CaravanArrivalAction_TransferPrisoners(MapParent mapParent, Quest_PrisonShipAccident quest)
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
            Map map = Find.AnyPlayerHomeMap;

            quest.CheckPrisoners();

            for(int i = quest.DropedPawns.Count - 1; i >= 0; i--)
            {
                Pawn p = quest.DropedPawns[i];
                if(caravan.ContainsPawn(p))
                {
                    caravan.pawns.Remove(p);
                    Find.WorldPawns.RemovePawn(p);
                    quest.DropedPawns.Remove(p);

                    IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
                    Thing rewardCopy = ThingMaker.MakeThing(ThingDefOf.Silver);
                    rewardCopy.stackCount = quest.Rewards[0].stackCount;

                    DropPodUtility.DropThingsNear(intVec, map, new List<Thing> { rewardCopy }, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

                    Find.LetterStack.ReceiveLetter("CaravanArrivalAction_TransferPrisoners_SuccessTitle".Translate(), "CaravanArrivalAction_TransferPrisoners_SuccessDesc".Translate(p.Name.ToStringFull, GssRaids.GssFaction.Name, quest.Rewards[0]), LetterDefOf.PositiveEvent, new LookTargets(intVec, map));
                }
            }

            if(quest.DropedPawns.Count == 0)
            {
                quest.Site.EndQuest(null, EndCondition.Success);
            }
        }
        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }

            return CanVisit(caravan);
        }

        public bool CanVisit(Caravan caravan)
        {
            return caravan.pawns.InnerListForReading.Any(x => quest.DropedPawns.Contains(x));
        }
    }
}
