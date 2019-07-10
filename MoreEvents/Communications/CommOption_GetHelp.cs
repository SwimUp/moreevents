using DiaRim;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_GetHelp : InteractionOption
    {
        public override string Label => "CommOption_GetHelp_Label".Translate(HelpCount);

        public int HelpCount = 0;

        public override Color TextColor => HelpCount == 0 ? Color.gray : Color.white;

        private FactionInteraction interaction;
        private Pawn speaker;

        public CommOption_GetHelp()
        {
        }

        public static void AddComponentWithStack(Faction faction, int addOrSetCount)
        {
            FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(faction);
            foreach (var option in interaction.Options)
            {
                if (option is CommOption_GetHelp optionGetHelp)
                {
                    optionGetHelp.HelpCount += addOrSetCount;
                    return;
                }
            }

            CommOption_GetHelp getHelp = new CommOption_GetHelp(addOrSetCount);
            interaction.Options.Add(getHelp);
        }

        public CommOption_GetHelp(int initialCount)
        {
            HelpCount = initialCount;
        }

        public override void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant)
        {
            if (HelpCount == 0)
            {
                return;
            }

            this.interaction = interaction;
            this.speaker = speaker;

            Dialog dialog = new Dialog(DialogDefOfLocal.CommOptionGetHelp, speaker);
            dialog.Init();
            dialog.CloseAction = CheckAnswer;
            Find.WindowStack.Add(dialog);
        }

        private void CheckAnswer(string answer)
        {
            if (answer == "конец")
                return;

            HelpCount--;

            int techLevel = (int)interaction.Faction.def.techLevel;
            bool useCaravan = techLevel < (int)TechLevel.Industrial;
            string caravanText = useCaravan ? "UsingCaravan".Translate() : "UsingCapsules".Translate();
            if (answer == "resources")
            {
                ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
                ThingSetMakerParams parms2 = default;
                parms2.totalMarketValueRange = new FloatRange(1000, 2000);
                parms2.filter = GetFilter();
                parms2.techLevel = interaction.Faction.def.techLevel;
                maker.fixedParams = parms2;

                if (useCaravan)
                {
                    AssistCaravanWithFixedInventoryComp assistComp = new AssistCaravanWithFixedInventoryComp(maker.Generate(), Rand.Range(1, 3) * 60000, interaction.Faction, speaker.Map);
                    assistComp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();
                    QuestsManager.Communications.RegisterComponent(assistComp);
                }
                else
                {
                    SendCapsules(maker.Generate());
                }

                Find.LetterStack.ReceiveLetter("Help_AskResoucesTitle".Translate(), "Help_AskResouces".Translate(interaction.Faction, caravanText), LetterDefOf.PositiveEvent);
            }
            if(answer == "power")
            {
                PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
                {
                    faction = interaction.Faction,
                    points = DiplomacyTuning.RequestedMilitaryAidPointsRange.RandomInRange,
                    generateFightersOnly = true,
                    groupKind = PawnGroupKindDefOf.Combat,
                    raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                    forceOneIncap = true
                };
                List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

                IncidentParms incidentParms = new IncidentParms();
                incidentParms.target = speaker.Map;
                incidentParms.faction = interaction.Faction;
                incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
                incidentParms.points = DiplomacyTuning.RequestedMilitaryAidPointsRange.RandomInRange;
                interaction.Faction.lastMilitaryAidRequestTick = Find.TickManager.TicksGame;

                if (useCaravan)
                {
                    incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                    Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidFriendly, Find.TickManager.TicksGame + 11000, incidentParms);
                }
                else
                {
                    IncidentDefOf.RaidFriendly.Worker.TryExecute(incidentParms);
                }

                Find.LetterStack.ReceiveLetter("Help_AskPowerTitle".Translate(), "Help_AskPower".Translate(interaction.Faction, caravanText), LetterDefOf.PositiveEvent);
            }
        }

        private ThingFilter GetFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);
            filter.SetAllow(ThingCategoryDefOf.Leathers, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);

            return filter;
        }

        private void SendCapsules(List<Thing> toDrop)
        {
            Map map = Find.AnyPlayerHomeMap;
            IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.DropThingsNear(intVec, map, toDrop, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref HelpCount, "HelpCount");
        }
    }
}
