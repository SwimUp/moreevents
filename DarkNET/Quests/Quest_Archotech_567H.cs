using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
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
    public class Quest_Archotech_567H : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => "";

        public override string Description => "";

        public override string PlaceLabel => "";

        public override bool HasExitCells => false;

        public override bool UseLeaveCommand => true;

        public virtual string MapTargetTag => "";

        public bool Won = false;

        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef MapGenerator;

        public virtual Faction FactionGetter => Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Pirate);

        public virtual float QuestChance => 0.65f;

        public virtual FloatRange RewardRange => new FloatRange(2000, 3000);

        public virtual IntRange CountRange => new IntRange(1, 2);

        public virtual IntRange DaysRange => new IntRange(6, 12);

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            MapGeneratorHandler.GenerateMap(MapGenerator, map, out List<Pawn> pawns, false, true, true, false, true, true, true, Faction);

            UnlimitedTime = true;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToQuestMapWithGenerator caravanAction = new CaravanArrivalAction_EnterToQuestMapWithGenerator(mapParent, MapGenerator);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            if (!Rand.Chance(QuestChance))
                return false;

            Faction faction = FactionGetter;
            if (faction == null)
                faction = Find.FactionManager.RandomEnemyFaction();

            Faction = faction;
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            GenerateRewards(GetQuestThingFilter(), RewardRange, CountRange, null, null);
            ShowInConsole = false;

            DefDatabase<MapGeneratorBlueprints.MapGenerator.MapGeneratorDef>.AllDefsListForReading.Where(gen => gen.targetTags != null && gen.targetTags.Contains(MapTargetTag)).TryRandomElementByWeight(w => w.Commonality, out MapGenerator);
            QuestsManager.Communications.AddQuest(this);

            return true;
        }

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if (HostileUtility.AnyNonDownedHostileOnMap(site.Map, Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            Won = true;

            return true;
        }

        public override void PostSiteRemove(QuestSite site)
        {
            base.PostSiteRemove(site);

            if (!Won)
                EndQuest(null, EndCondition.Fail);
            else
                EndQuest(null, EndCondition.Success);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Won, "Won");
            Scribe_Defs.Look(ref MapGenerator, "MapGenerator");
        }

        public override void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 5, 12, out int newTile, (int i) => !Find.WorldObjects.AnyWorldObjectAt(i));

            LookTargets target = new LookTargets(newTile);
            Target = target;

            QuestSite questPlace = QuestsHandler.CreateSiteFor(this, newTile, Faction);

            Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            ShowInConsole = true;

            TicksToPass = DaysRange.RandomInRange * 60000;

            if (notify)
            {
                Find.LetterStack.ReceiveLetter(QuestsManager.Communications.MakeQuestLetter(this, "TakeQuestFromPawnLetter".Translate(CardLabel), "TakeQuestFromPawnLetterDesc".Translate(CardLabel, Description)));
            }
        }
    }
}
