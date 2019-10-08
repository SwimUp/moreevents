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
    public class Quest_Archotech_567H_KillAll : Quest_Archotech_567H
    {
        public override string ExpandingIconPath => "Quests/Quest_Archotech_567H_KillAll";

        public override string CardLabel => "Quest_Archotech_567H_KillAll_CardLabel".Translate();

        public override string Description => "Quest_Archotech_567H_KillAll_Description".Translate();

        public override string PlaceLabel => "Quest_Archotech_567H_KillAll_PlaceLabel".Translate();

        public override string MapTargetTag => "Quest_Archotech_567H_KillAll";

        public override float QuestChance => 0.65f;

        public override Faction FactionGetter => Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Pirate);

        public override FloatRange RewardRange => new FloatRange(1000, 1500);

        public override IntRange CountRange => new IntRange(1, 2);

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Weapons, true);

            return filter;
        }
    }
}
