using DarkNET.Caravans;
using DarkNET.Traders;
using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using QuestRim;
using RimOverhaul.AI;
using RimOverhaul.Things;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DarkNET.Quests
{
    public class Quest_Laboratory : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => "Quest_Laboratory_CardLabel".Translate();

        public override string Description => "Quest_Laboratory_Description".Translate();

        public override string PlaceLabel => "Quest_Laboratory_PlaceLabel".Translate();

        public override string ExpandingIconPath => "Quests/Quest_Laboratory";

        public Building_LaboratoryConsole Console;

        public override bool UseLeaveCommand => false;
        public override bool HasExitCells => true;

        private IntVec3[] destroyableItems = new IntVec3[]
        {
            new IntVec3(9, 0, 235),
            new IntVec3(9, 0, 232),
            new IntVec3(10, 0, 231),
            new IntVec3(8, 0, 230),
            new IntVec3(9, 0, 229),
            new IntVec3(8, 0, 227),
            new IntVec3(8, 0, 226),
            new IntVec3(8, 0, 224),
            new IntVec3(8, 0, 223),
            new IntVec3(8, 0, 222)
        };

        public Quest_Laboratory()
        {

        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToLaboratoryMap caravanAction = new CaravanArrivalAction_EnterToLaboratoryMap(mapParent, this);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "Quest_Laboratory_CaravanArrivalAction_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            MapGeneratorHandler.GenerateMap(MapDefOfLocal.BaseX1, map, out List<Pawn> pawns, true, true, true, false, true, true, true, Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Pirate));

            var mutantsFaction = Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.Mutants);
            if (mutantsFaction == null)
                Log.Warning("Mutants is null.");

            foreach(var p in pawns)
            {
                if(p.RaceProps.Animal)
                {
                    p.GetLord()?.Notify_PawnLost(p, PawnLostCondition.Undefined, null);

                    p.SetFaction(mutantsFaction);

                    LordJob_AnimalDefendPointAggresive lordJob = new LordJob_AnimalDefendPointAggresive(p.Faction, p.Position);
                    Lord lord = LordMaker.MakeNewLord(p.Faction, lordJob, map);
                    lord.numPawnsLostViolently = int.MaxValue;

                    lord.AddPawn(p);
                }
            }

            DiaNode node = new DiaNode("Quest_Laboratory_Info1".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);

            UnlimitedTime = true;

            DoDestroyDestroyable();
        }

        private void DoDestroyDestroyable()
        {
            foreach(var itemPos in destroyableItems)
            {
                List<Thing> items = itemPos.GetThingList(Site.Map);

                foreach(var item in items)
                {
                    CompRefuelable compRefuelable = item.TryGetComp<CompRefuelable>();
                    if (compRefuelable != null)
                    {
                        float refuelInt = Mathf.Clamp(compRefuelable.Fuel - compRefuelable.Props.fuelCapacity, 0, compRefuelable.Props.fuelCapacity);

                        compRefuelable.Refuel(refuelInt);
                    }

                    CompBreakdownable compBreakdownable = item.TryGetComp<CompBreakdownable>();
                    if(compBreakdownable != null)
                    {
                        compBreakdownable.DoBreakdown();
                    }

                    if(item.def.CanHaveFaction)
                        item.SetFaction(Faction.OfPlayer);
                }
            }
        }

        public void FindConsole()
        {
            if (Site != null && Site.HasMap)
            {
                Console = Site.Map.listerThings.AllThings.Where(x => x is Building_LaboratoryConsole).FirstOrDefault() as Building_LaboratoryConsole;
            }
            else
            {
                Console = null;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Site != null && Site.HasMap)
            {
                if (Console == null)
                    FindConsole();

                if (Console != null)
                {
                    if (Console.Used)
                    {
                        EndQuest(null, EndCondition.Success);
                        Site.RemoveAfterLeave = true;
                    }
                }
            }
        }

        public override void DrawAdditionalOptions(Rect rect)
        {
            Widgets.Label(rect, "Quest_Laboratory_AdditionalRewards".Translate());
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if(condition == EndCondition.Timeout)
            {
                IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.World);
                parms.forced = true;

                var incident = new FiringIncident(MoreEvents.IncidentDefOfLocal.HighMutantPopulation, null, parms);
                Find.Storyteller.TryFire(incident);
            }

            base.EndQuest(caravan, condition);
        }

        public override void GiveRewards(Caravan caravan)
        {
            base.GiveRewards(caravan);

            DarkNet net = Current.Game.GetComponent<DarkNet>();
            TraderWorker_Eisenberg eisenberg = (TraderWorker_Eisenberg)net.Traders.First(x => x is TraderWorker_Eisenberg);

            eisenberg.Reputation = Mathf.Clamp(eisenberg.Reputation + 20, -100, 100);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref Console, "ConsoleRef");
        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.Medicine, true);
            filter.SetAllow(ThingCategoryDefOf.Drugs, true);
            filter.SetAllow(ThingCategoryDefOfLocal.DarkNetItems_Drugs, true);

            return filter;
        }
    }
}
