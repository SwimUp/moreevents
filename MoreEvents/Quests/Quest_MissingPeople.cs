using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Quests
{
    public class Quest_MissingPeople : Quest
    {
        public override string CardLabel => "Quest_MissingPeople_CardLabel".Translate();

        public override string Description => "Quest_MissingPeople_Description".Translate(Faction.Name, minDays, passedDays, TicksToPass.TicksToDays().ToString("f2"));

        public override string PlaceLabel => "Quest_MissingPeople_PlaceLabel".Translate();

        public override string ExpandingIconPath => "Quests/Quest_MissingPeople";

        public float FindChance => 0.35f;
        public int BaseTile;
        private int minDays = 5;
        private int passedDays = 8;

        private List<Pawn> savedPawns = new List<Pawn>();

        public Quest_MissingPeople()
        {

        }

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();

            GenerateAlliedPawns();
        }

        private void GenerateAlliedPawns()
        {
            for(int i = 0; i < Rand.Range(2, 4); i++)
            {
                Pawn p = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction);
                savedPawns.Add(p);
            }
        }

        public Quest_MissingPeople(int daysLeft, int minDays = 5, int passedDays = 8)
        {
            TicksToPass = daysLeft * 60000;
            this.minDays = minDays;
            this.passedDays = passedDays;
        }

        public override void Tick()
        {

        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_CheckMissingPeople caravanAction = new CaravanArrivalAction_CheckMissingPeople(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "Quest_CheckMissingPeople_Action".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);
            filter.SetAllow(ThingCategoryDefOf.Manufactured, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);

            return filter;
        }

        public override void SiteTick()
        {
            TicksToPass--;

            if(TicksToPass <= 0)
            {
                EndQuest(null, EndCondition.Fail);
            }
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            base.EndQuest(caravan, condition);

            if(condition == EndCondition.Fail || condition == EndCondition.Timeout)
            {
                Faction.TryAffectGoodwillWith(Faction.OfPlayer, -10);
            }
        }

        public override string GetInspectString()
        {
            return "Quest_MissingPeople_InspectString".Translate(TicksToPass.TicksToDays().ToString("f2"));
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref savedPawns, "savedPawns", LookMode.Reference);
            Scribe_Values.Look(ref BaseTile, "baseTile");
            Scribe_Values.Look(ref minDays, "minDays");
            Scribe_Values.Look(ref passedDays, "passedDays");
        }
    }
}
