using MapGeneratorBlueprints.MapGenerator;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Quests
{
    public class Quest_KillLeader : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_KillLeader;

        public override string CardLabel => "Quest_KillLeader_CardLabel".Translate();

        public override string Description => "Quest_KillLeader_Description".Translate(Faction.Name, TargetPawn.Name.ToStringFull, TargetPawn.Faction.Name);

        public override string ExpandingIconPath => "Quests/Quest_KillLeader";

        public override string PlaceLabel => "Quest_KillLeader_PlaceLabel".Translate();

        public Pawn TargetPawn;

        public string MapGenerator => "Quest_KillLeader";

        private bool Won = false;

        public Quest_KillLeader()
        {

        }

        public Quest_KillLeader(Pawn target, int daysToPass)
        {
            TargetPawn = target;
            TicksToPass = daysToPass * 60000;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToKillLeaderMap caravanAction = new CaravanArrivalAction_EnterToKillLeaderMap(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            DefDatabase<MapGeneratorBlueprints.MapGenerator.MapGeneratorDef>.AllDefsListForReading.Where(gen => gen.targetTags != null && gen.targetTags.Contains(MapGenerator)).TryRandomElementByWeight(w => w.Commonality, out MapGeneratorBlueprints.MapGenerator.MapGeneratorDef result);

            MapGeneratorHandler.GenerateMap(result, map, out List<Pawn> pawns, true, true, true, false, true, true, true, TargetPawn.Faction);

            TargetPawn = (Pawn)GenSpawn.Spawn(TargetPawn, pawns.RandomElement().Position, map);
            pawns[0].GetLord().AddPawn(TargetPawn);
        }

        public override void SiteTick()
        {
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                CheckWon();
            }
        }

        public override bool CanLeaveFromSite(QuestSite site)
        {
            if (TargetPawn != null && !TargetPawn.Dead)
            {
                Messages.Message("LeaderStillAlive".Translate(TargetPawn.Name.ToStringFull), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (AnyHostileOnMap(site.Map, TargetPawn.Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        private bool AnyHostileOnMap(Map map, Faction enemyFaction)
        {
            List<Pawn> enemyPawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == enemyFaction && !p.Dead && p.RaceProps.Humanlike).ToList();

            if (enemyPawns == null || enemyPawns.Count == 0)
                return false;

            return true;
        }

        private void CheckWon()
        {
            if (!Won && TargetPawn.Dead)
            {
                UnlimitedTime = true;
                Won = true;
                Find.LetterStack.ReceiveLetter("Quest_KillLeaderTitle".Translate(), "Quest_KillLeaderDesc".Translate(), LetterDefOf.PositiveEvent);
            }

            if (!Site.HasMap && Won)
            {
                Site.EndQuest(null, EndCondition.Success);
            }
        }

        public override void PostSiteRemove(QuestSite site)
        {
            CheckWon();

            if (!Won)
                Find.WorldPawns.PassToWorld(TargetPawn);
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Apparel, true);
            filter.SetAllow(ThingCategoryDefOf.Items, true);
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);

            return filter;
        }

        public override bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            EventSettings settings = Settings.EventsSettings["Quest_KillLeader"];
            if (!settings.Active)
                return false;

            if (!TryResolveTwoFactions(out Faction alliedFaction, out Faction enemyFaction))
                return false;

            if (enemyFaction.leader == null)
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 8, 20, out int result))
                return false;

            Faction = alliedFaction;
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
            GenerateRewards(GetQuestThingFilter(), new FloatRange(600, 800) * (float)enemyFaction.def.techLevel, new IntRange(1, 3), null, null);

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = result;
            questPlace.SetFaction(alliedFaction);
            questPlace.Init(this);
            questPlace.RemoveAfterLeave = false;

            Target = questPlace;
            Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            ShowInConsole = false;

            QuestsManager.Communications.AddQuestPawn(questPawn, this);
            QuestsManager.Communications.AddQuest(this);

            return true;
        }

        private bool TryResolveTwoFactions(out Faction alliedFaction, out Faction enemyFaction)
        {
            enemyFaction = null;
            alliedFaction = null;

            Faction faction1 = Find.FactionManager.RandomNonHostileFaction();
            if (faction1 == null)
                return false;

            if (Find.FactionManager.AllFactionsVisible.Where(f => f != faction1 && f.HostileTo(faction1) && f.HostileTo(Faction.OfPlayer)).TryRandomElement(out enemyFaction))
            {
                alliedFaction = faction1;
                return true;
            }

            return false;
        }

        public override string GetInspectString()
        {
            return "InspectString_Timer".Translate(TicksToPass.TicksToDays().ToString("f2"));
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref TargetPawn, "TargetPawn");
        }
    }
}
