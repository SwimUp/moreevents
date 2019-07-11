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
    public class Quest_ThingsHelp : Quest
    {
        private Vector2 slider = Vector2.zero;

        public override string AdditionalQuestContentString => "Quest_RequiredItems".Translate();

        public override string CardLabel => "Quest_HelpItems_Label".Translate();

        public override string Description => "Quest_HelpItems_Description".Translate(Faction.Name);

        public override string PlaceLabel => "Quest_HelpItems_Site".Translate();

        public override string ExpandingIconPath => "Quests/Quest_HelpResources";

        public Dictionary<ThingDef, int> RequestItems = new Dictionary<ThingDef, int>();

        public override void DrawAdditionalOptions(Rect rect)
        {
            Rect rect2 = rect;
            int requestSliderLength = RequestItems.Values.Count * 30;
            Rect requestRect = new Rect(0, 0, rect.width, requestSliderLength);
            Rect scrollRewVertRectFact = new Rect(0, 0, rect2.x, requestSliderLength);
            Widgets.BeginScrollView(rect2, ref slider, scrollRewVertRectFact, false);
            DrawRequestItems(requestRect);
            Widgets.EndScrollView();
        }
        
        private void DrawRequestItems(Rect requestRect)
        {
            Text.Font = GameFont.Small;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(requestRect);
            foreach (var item in RequestItems)
            {
                listing.Label($"{item.Key.LabelCap} - {item.Value}x", 25);
            }
            listing.End();
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

        public override string GetInspectString()
        {
            return "HowToCheckReqList".Translate();
        }

        public override string GetDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("HelpResourcesDescriptionTitle".Translate(Faction));
            foreach (var item in RequestItems)
            {
                builder.Append($"\n- {item.Key.LabelCap} - {item.Value}x");
            }

            return builder.ToString();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_GiveItems caravanAction = new CaravanArrivalAction_GiveItems(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanGiveItems(caravan, mapParent), () => caravanAction, "GiveItemsOption".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if(condition == EndCondition.Timeout)
            {
                Faction.TryAffectGoodwillWith(Faction.OfPlayer, -10);
            }

            base.EndQuest(caravan, condition);
        }

        public override void GiveRewards(Caravan caravan)
        {
            int level = (int)TechLevel.Spacer;
            if ((int)Faction.def.techLevel >= level)
            {
                base.GiveRewards(caravan);

                Find.LetterStack.ReceiveLetter("GiveItemsSuccessTitle".Translate(), "GiveItemsSuccess".Translate(), LetterDefOf.PositiveEvent);
            }
            else
            {
                if (caravan != null)
                {
                    foreach (var reward in Rewards)
                    {
                        CaravanInventoryUtility.GiveThing(caravan, reward);
                    }

                    Find.LetterStack.ReceiveLetter("GiveItemsSuccessTitle".Translate(), "GiveItemsSuccess2".Translate(), LetterDefOf.PositiveEvent);
                }
            }

            Faction.TryAffectGoodwillWith(Faction.OfPlayer, 15);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref RequestItems, "RequestItems", LookMode.Def, LookMode.Value);
        }
    }
}
