using MoreEvents;
using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Quests
{
    public class Alliance_Quest_ResearchSharing : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => IncidentDef.LabelCap;

        public override string Description => string.Format(IncidentDef.letterText, Faction.Name, PointsReward);

        public SkillDef SkillDef;

        public int PointsReward;

        public bool Started;

        public override string PlaceLabel => IncidentDef.LabelCap;

        public override int SuccessTrustAffect => 10;

        public override int TimeoutTrustAffect => -5;

        public override int FailTrustAffect => -5;

        public override string ExpandingIconPath => "Map/Alliance_Quest_ResearchSharing";

        public Pawn Pawn;

        public Alliance_Quest_ResearchSharing()
        {

        }

        public Alliance_Quest_ResearchSharing(Faction faction, SkillDef skillDef, int pointsReward)
        {
            Faction = faction;
            SkillDef = skillDef;
            PointsReward = pointsReward;
        }

        public void StartResearchSharing(Pawn pawn)
        {
            TicksToPass = 2 * 60000;
            Started = true;
            Pawn = pawn;

            Find.LetterStack.ReceiveLetter("Alliance_Quest_ResearchSharing_StartedTitle".Translate(), "Alliance_Quest_ResearchSharing_StartedDesc".Translate(), LetterDefOf.PositiveEvent);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_StartResearchSharing action = new CaravanArrivalAction_StartResearchSharing(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => action.CanUse(caravan), () => action, CardLabel, caravan, mapParent.Tile, mapParent);
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if (Started)
                condition = EndCondition.Success;

            if(Pawn != null)
                CaravanMaker.MakeCaravan(new List<Pawn> { Pawn }, RimWorld.Faction.OfPlayer, Site.Tile, false);

            if (condition == EndCondition.Success)
            {
                var project = Find.ResearchManager.currentProj;
                if (project != null)
                {
                    ReserachUtility.AddPoints(project, PointsReward);
                }
            }

            base.EndQuest(caravan, condition);
        }

        public override string GetDescription()
        {
            return Description;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref SkillDef, "SkillDef");
            Scribe_Values.Look(ref PointsReward, "PointsReward");
            Scribe_Values.Look(ref Started, "Started");
            Scribe_References.Look(ref Pawn, "Pawn");
        }
    }
}
