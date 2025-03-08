﻿using MapGeneratorBlueprints.MapGenerator;
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
    public class Quest_KillOrder : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_KillLeader;

        public override string CardLabel => "Quest_KillOrder_CardLabel".Translate();

        public override string Description => "Quest_KillOrder_Description".Translate(Faction.Name, TargetPawn.Name.ToStringFull, TargetPawn.Faction.Name);

        public override string ExpandingIconPath => "Quests/Quest_KillOrder";

        public override string PlaceLabel => "Quest_KillOrder_PlaceLabel".Translate();

        public Pawn TargetPawn;

        public string MapGenerator => "Quest_KillOrder";

        private bool Won = false;

        public override bool UseLeaveCommand => false;

        public override bool HasExitCells => true;

        public Quest_KillOrder()
        {

        }

        public Quest_KillOrder(Pawn target, int daysToPass)
        {
            TargetPawn = target;
            TicksToPass = daysToPass * 60000;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToKillOrderMap caravanAction = new CaravanArrivalAction_EnterToKillOrderMap(mapParent, this);
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
            //pawns[0].GetLord().AddPawn(TargetPawn);

            UnlimitedTime = true;

            Site.RemoveAfterLeave = false;
            Site.RemoveIfAllDie = true;
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
                Messages.Message("OrderedTargetStillAlive".Translate(TargetPawn.Name.ToStringFull), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (TargetPawn != null && HostileUtility.AnyNonDownedHostileOnMap(site.Map, TargetPawn.Faction))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        private void CheckWon()
        {
            if (!Won && (TargetPawn == null || TargetPawn.Dead))
            {
                UnlimitedTime = true;
                Won = true;
                Site.RemoveAfterLeave = true;
                Find.LetterStack.ReceiveLetter("Quest_KillOrderTitle".Translate(), "Quest_KillOrderDesc".Translate(TargetPawn.Name.ToStringFull), LetterDefOf.PositiveEvent);
            }

            if (!Site.HasMap && Won)
            {
                Site.EndQuest(null, EndCondition.Success);
            }
        }

        public override void PostMapRemove(Map map)
        {
            CheckWon();

            if (!Won && !TargetPawn.IsWorldPawn())
                Find.WorldPawns.PassToWorld(TargetPawn);
        }

        public override void GenerateRewards()
        {
            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = Rand.Range(100, 300) * (int)TargetPawn.Faction.def.techLevel;

            Rewards = new List<Thing>
            {
                silver
            };
        }

        public override bool TryGiveQuestTo(Pawn questPawn, QuestDef questDef)
        {
            if (questPawn == null)
                return false;

            EventSettings settings = Settings.EventsSettings["Quest_KillOrder"];
            if (!settings.Active)
                return false;

            if (!TryResolveTwoFaction(out Faction faction1, out Faction faction2))
                return false;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 8, 24, out int result))
                return false;

            Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier, faction2);
            Find.WorldPawns.PassToWorld(pawn);

            if (pawn == null)
                return false;

            TargetPawn = pawn;

            Faction = faction1;
            TicksToPass = Rand.Range(6, 12) * 60000;
            id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();

            GenerateRewards();

            ShowInConsole = false;

            QuestsManager.Communications.AddQuestPawn(questPawn, this);
            QuestsManager.Communications.AddQuest(this);

            if (TargetPawn == null)
                return false;

            return true;
        }

        public override void TakeQuestByQuester(QuestPawn quester, bool notify = true)
        {
            TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, 6, 24, out int result);

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

        private bool TryResolveTwoFaction(out Faction faction1, out Faction faction2)
        {
            faction2 = null;

            faction1 = Find.FactionManager.RandomNonHostileFaction();
            if (faction1 == null)
                return false;

            Faction faction3 = faction1;
            if (Find.FactionManager.AllFactionsVisible.Where(f => f != Faction.OfPlayer && f.HostileTo(faction3)).TryRandomElement(out faction2))
            {
                return true;
            }

            return false;
        }

        public override string GetInspectString()
        {
            string text = "InspectString_Timer".Translate(TicksToPass.TicksToDays().ToString("f2"));
            text += $"\n{"Quest_KillOrderTargetInfo".Translate(TargetPawn.Name.ToStringFull, Rewards[0].LabelCap)}";
            return text;
        }

        public override string GetDescription()
        {
            string text = "Quest_KillOrder_CardLabel".Translate();
            text += $"\n{"Quest_KillOrderTargetInfo".Translate(TargetPawn.Name.ToStringFull, Rewards[0].LabelCap)}";

            return text;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref TargetPawn, "TargetPawn");
            Scribe_Values.Look(ref Won, "Won");
        }
    }
}
