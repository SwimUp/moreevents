using MoreEvents.Communications;
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
    public class Quest_BuildNewBase : Quest
    {
        public override string CardLabel => "Quest_BuildNewBase_Label".Translate();

        public override string Description => "Quest_BuildNewBase_Description".Translate(Faction.Name, Faction.leader.Name.ToStringFull);

        public override string PlaceLabel => "Quest_BuildNewBase_Place".Translate(Faction);

        public override string ExpandingIconPath => "Quests/Quest_BuildNewBase";

        public int PawnsRequired;
        public int TicksToEnd;
        public bool Entered = false;
        public List<Pawn> EnteredPawns = new List<Pawn>();
        public QuestSite Site;
        public Settlement OldSettlement;

        public override void SiteTick()
        {
            if(Entered)
            {
                TicksToEnd--;

                if(TicksToEnd <= 0)
                {
                    Site.EndQuest(null, EndCondition.Success);
                }
            }
        }

        public override ThingFilter GetQuestThingFilter()
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

        public override string GetInspectString()
        {
            return "HowToGivePawns".Translate(PawnsRequired, GenDate.TicksToDays(TicksToEnd).ToString("f2"));
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_HelpWithBuildings caravanAction = new CaravanArrivalAction_HelpWithBuildings(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanHelp(caravan, mapParent), () => caravanAction, "Quest_HelpWithBuilding_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            base.EndQuest(caravan, condition);

            if (condition == EndCondition.Success)
            {
                CaravanMaker.MakeCaravan(EnteredPawns, RimWorld.Faction.OfPlayer, Site.Tile, false);

                Find.LetterStack.ReceiveLetter("BuildingSeccessEndTitle".Translate(), "BuildingSeccessEnd".Translate(Faction.Name), LetterDefOf.PositiveEvent);

                CommOption_GetHelp.AddComponentWithStack(Faction, 1);

                Faction.TryAffectGoodwillWith(Faction.OfPlayer, 20);

                OldSettlement.Tile = Site.Tile;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref Site, "Site");
            Scribe_References.Look(ref OldSettlement, "OldSettlement");
            Scribe_Collections.Look(ref EnteredPawns, "Pawns", LookMode.Reference);
            Scribe_Values.Look(ref Entered, "Entered");
            Scribe_Values.Look(ref TicksToEnd, "TicksToEnd");
            Scribe_Values.Look(ref PawnsRequired, "PawnsRequired");
        }

        public override void DrawAdditionalOptions(Rect rect)
        {
            Widgets.Label(rect, "PawnsRequired".Translate(PawnsRequired));
        }
    }
}
