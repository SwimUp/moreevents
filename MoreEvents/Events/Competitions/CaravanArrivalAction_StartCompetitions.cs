using MoreEvents.Quests;
using QuestRim;
using RimOverhaul.Events.Competitions;
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
    public class CaravanArrivalAction_StartCompetitions : CaravanArrivalAction
    {
        public MapParent MapParent => mapParent;
        private MapParent mapParent;

        public WorldObject_Competitions WorldObject_Competitions => (WorldObject_Competitions)mapParent;

        public override string Label => "Competitions_Label".Translate();

        public override string ReportString => "Competitions_ReportString".Translate();

        public CaravanArrivalAction_StartCompetitions()
        {
        }

        public CaravanArrivalAction_StartCompetitions(MapParent mapParent)
        {
            this.mapParent = mapParent;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref mapParent, "Parent");
        }

        public override void Arrived(Caravan caravan)
        {
            WorldObject_Competitions.PlayerPawn = caravan.pawns.InnerListForReading.First(x => x.skills.GetSkill(WorldObject_Competitions.CompetitionSkill).Level >= WorldObject_Competitions.CompetitionSkillLevelRequired);

            caravan.RemovePawn(WorldObject_Competitions.PlayerPawn);

            if (caravan.pawns.Count == 0)
                Find.WorldObjects.Remove(caravan);

            WorldObject_Competitions.StartCompetitions();
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }

            return CanVisit(caravan);
        }

        public FloatMenuAcceptanceReport CanVisit(Caravan caravan)
        {
            if (WorldObject_Competitions.CompStarted)
                return false;

            if (!caravan.pawns.InnerListForReading.Any(x => x.skills.GetSkill(WorldObject_Competitions.CompetitionSkill).Level >= WorldObject_Competitions.CompetitionSkillLevelRequired))
                return FloatMenuAcceptanceReport.WithFailReason("NotEnoughtSkillForComp".Translate());

            return true;
        }
    }
}
