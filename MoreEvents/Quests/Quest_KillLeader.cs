using MapGeneratorBlueprints.MapGenerator;
using QuestRim;
using RimOverhaul;
using RimOverhaul.AI;
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

        public override int SuccessTrustAffect => 25;

        public override int FailTrustAffect => -5;

        public override int TimeoutTrustAffect => -10;

        public override int SuccessAggressiveLevelAffect => 10;

        public override int FailAggressiveLevelAffect => 10;

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

            Lord defendLord = LordMaker.MakeNewLord(TargetPawn.Faction, new LordJob_DefendPawn(TargetPawn), map);
            defendLord.numPawnsLostViolently = int.MaxValue;

            MapGeneratorHandler.GenerateMap(result, map, out List<Pawn> pawns, true, true, true, false, true, true, true, TargetPawn.Faction, defendLord);

            TargetPawn = (Pawn)GenSpawn.Spawn(TargetPawn, pawns.RandomElement().Position, map);
            Lord leaderLord = LordMaker.MakeNewLord(TargetPawn.Faction, new LordJob_DefendPoint(TargetPawn.Position), map);
            leaderLord.AddPawn(TargetPawn);

            UnlimitedTime = true;
        }

        public override string GetDescription()
        {
            StringBuilder builder = new StringBuilder();
            foreach(var reward in Rewards)
            {
                builder.AppendLine(reward.LabelCap);
            }

            string text = "Quest_KillLeader_CardLabel".Translate();
            text += $"\n{"Quest_KillOrderTargetInfo".Translate(TargetPawn.Name.ToStringFull, builder.ToString())}";

            return text;
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

            if (TargetPawn != null && site.Map != null && HostileUtility.AnyNonDownedHostileOnMap(site.Map, TargetPawn.Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

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

        public override void PostMapRemove(Map map)
        {
            CheckWon();

            UnlimitedTime = false;

            if (!Won && !TargetPawn.IsWorldPawn())
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
            TicksToPass = Rand.Range(8, 15) * 60000;
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();

            TargetPawn = enemyFaction.leader;

            GenerateRewards(GetQuestThingFilter(), new FloatRange(600, 800) * (float)enemyFaction.def.techLevel, new IntRange(1, 3), null, null);

            ShowInConsole = false;

            QuestsManager.Communications.AddQuestPawn(questPawn, this);
            QuestsManager.Communications.AddQuest(this);

            return true;
        }

        public override void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 8, 20, out int result);

            QuestSite questPlace = (QuestSite)WorldObjectMaker.MakeWorldObject(QuestRim.WorldObjectDefOfLocal.QuestPlace);
            questPlace.Tile = result;
            questPlace.SetFaction(Faction);
            questPlace.Init(this);
            questPlace.RemoveAfterLeave = false;

            Target = questPlace;
            Site = questPlace;

            Find.WorldObjects.Add(questPlace);

            base.TakeQuestByQuester(quester, notify);
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
            Scribe_Values.Look(ref Won, "Won");
        }
    }
}
