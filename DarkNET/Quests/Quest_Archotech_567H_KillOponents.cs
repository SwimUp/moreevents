using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimOverhaul;
using RimOverhaul.Quests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Quests
{
    public class Quest_Archotech_567H_KillOponents : Quest_Archotech_567H
    {
        public override string ExpandingIconPath => "Quests/Quest_Archotech_567H_KillOponents";

        public override string CardLabel => "Quest_Archotech_567H_KillOponents_CardLabel".Translate();

        public override string Description => "Quest_Archotech_567H_KillOponents_Description".Translate();

        public override string PlaceLabel => "Quest_Archotech_567H_KillOponents_PlaceLabel".Translate();

        public override string MapTargetTag => "Quest_Archotech_567H_KillOponents";

        public override float QuestChance => 0.55f;

        public override Faction FactionGetter => Find.FactionManager.RandomEnemyFaction();

        public override FloatRange RewardRange => new FloatRange(2500, 3000);

        public override IntRange CountRange => new IntRange(1, 3);

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if (HostileUtility.AnyNonDeadHostileOnMap(site.Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            Won = true;

            return true;
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);

            return filter;
        }
    }
}
