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
    public class CaravanArrivalAction_HelpWithBuildings : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public Quest_BuildNewBase Quest => quest;
        private Quest_BuildNewBase quest;

        public override string Label => "Quest_HelpWithBuilding_Option".Translate();

        public override string ReportString => "BuildingSeccessEndTitle".Translate();

        public CaravanArrivalAction_HelpWithBuildings()
        {
        }

        public CaravanArrivalAction_HelpWithBuildings(MapParent mapParent, Quest_BuildNewBase quest)
        {
            this.mapParent = mapParent;
            this.quest = quest;
        }

        public override void Arrived(Caravan caravan)
        {
            for(int i = 0; i < quest.PawnsRequired; i++)
            {
                Pawn pawn = caravan.PawnsListForReading[i];

                quest.EnteredPawns.Add(pawn);
                caravan.RemovePawn(pawn);
            }

            quest.Entered = true;

            Find.LetterStack.ReceiveLetter("HelpStartedTitle".Translate(), "HelpStarted".Translate(quest.TicksToEnd.TicksToDays().ToString("f2")), LetterDefOf.PositiveEvent);
        }

        public virtual FloatMenuAcceptanceReport CanHelp(Caravan caravan, MapParent mapParent)
        {
            if (quest.Entered)
            {
                return FloatMenuAcceptanceReport.WithFailReason("BuildingAlreadyStarted".Translate());
            }

            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }

            if (caravan.pawns.Count < quest.PawnsRequired)
            {
                return FloatMenuAcceptanceReport.WithFailReason("NotEnoughPawnsToHelp".Translate());
            }

            return true;
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if(quest.Entered)
            {
                return FloatMenuAcceptanceReport.WithFailReason("BuildingAlreadyStarted".Translate());
            }

            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return FloatMenuAcceptanceReport.WithFailReason("NotEnoughPawnsToHelp".Translate());
            }

            return CanHelp(caravan, mapParent);
        }
    }
}
