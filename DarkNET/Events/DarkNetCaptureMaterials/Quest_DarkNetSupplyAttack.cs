using MoreEvents;
using QuestRim;
using RimOverhaul;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DarkNET.Events.DarkNetCaptureMaterials
{
    public class Quest_DarkNetCaptureMaterials : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => string.Format(IncidentDefOfLocal.DarkNetCaptureMaterials.letterLabel, TraderDef.LabelCap);

        public override string Description => string.Format(IncidentDefOfLocal.DarkNetCaptureMaterials.letterText, TraderDef.LabelCap);

        public override string PlaceLabel => "Quest_DarkNetCaptureMaterials_CardLabel".Translate(TraderDef.LabelCap);

        public DarkNetTraderDef TraderDef;

        public override bool UseLeaveCommand => false;

        public override bool HasExitCells => true;

        private bool signalSended = false;

        public DarkNetTrader DarkNetTrader
        {
            get
            {
                if (darkNetTrader == null)
                {
                    darkNetTrader = Current.Game.GetComponent<DarkNet>().Traders.First(x => x.def == TraderDef);
                }

                return darkNetTrader;
            }
        }

        private PawnKindDef[] allowedCarriers = new PawnKindDef[]
        {
            PawnKindDefOf.Muffalo,
            PawnKindDefOfLocal.Alpaca,
            PawnKindDefOfLocal.Dromedary
        };

        public override string ExpandingIconPath => "Quests/Quest_DarkNetCaptureMaterials";

        private DarkNetTrader darkNetTrader;

        private List<Thing> items = new List<Thing>();

        public Quest_DarkNetCaptureMaterials() : base()
        {

        }

        public Quest_DarkNetCaptureMaterials(DarkNetTraderDef traderDef)
        {
            TraderDef = traderDef;
        }

        public override void DrawAdditionalOptions(Rect rect)
        {
            Widgets.Label(rect, "Quest_DarkNetCaptureMaterials_AdditionalContent".Translate());
        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            GeneratePawns(map, Rand.Range(700, 1600));

            ShowDialog();
        }

        private void ShowDialog()
        {
            DiaNode node = new DiaNode("Quest_DarkNetCaptureMaterials_Info".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        private void GeneratePawns(Map map, float points)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = Faction,
                points = points,
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack,
                forceOneIncap = true
            };

            var pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            GenerateItems();
            foreach(var carrier in GenerateCarriers())
            {
                pawns.Add(carrier);
            }

            IncidentParms parms = new IncidentParms
            {
                target = map,
                spawnCenter = CellFinder.RandomClosewalkCellNear(map.Center, map, 15, x => x.Walkable(map) && !x.Fogged(map))
            };

            foreach(var pawn in pawns)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 8);
                GenSpawn.Spawn(pawn, loc, map, parms.spawnRotation);
            }

            LordJob_DefendBase lordJob = new LordJob_DefendBase(Faction, parms.spawnCenter);
            Lord lord = LordMaker.MakeNewLord(Faction, lordJob, map, pawns);

        }

        public override ThingFilter GetQuestThingFilter()
        {
            ThingFilter thingFilter = new ThingFilter();
            thingFilter.SetAllow(ThingCategoryDefOf.Root, true);

            return thingFilter;
        }

        public override IEnumerable<Gizmo> GetGizmos(QuestSite site)
        {
            foreach (var gizmo in base.GetGizmos(site))
            {
                yield return gizmo;
            }

            if (site.HasMap)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Quest_Archotech_567H_GetResources_SignalLabel".Translate(),
                    defaultDesc = "Quest_Archotech_567H_GetResources_SignalDescription".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Quests/send-signal"),
                    action = delegate
                    {
                        if (HostileUtility.AnyNonDownedHostileOnMap(site.Map, Faction))
                        {
                            Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                            return;
                        }

                        EndQuestAndGenerateRewards();
                    }
                };
            }
        }

        private void EndQuestAndGenerateRewards()
        {
            float rewardValue = 0;
            foreach (var item in items)
            {
                if(item != null && !item.Destroyed)
                {
                    rewardValue += item.stackCount * item.MarketValue;

                    item.Destroy();
                }
            }
            rewardValue *= Rand.Range(0.8f, 1.2f);

            GenerateRewards(GetQuestThingFilter(), new FloatRange(rewardValue, rewardValue), new IntRange(3, 8), null, null);

            DiaNode node = new DiaNode("Quest_DarkNetCaptureMaterials_SignalDialog".Translate(GetRewardsString()));
            DiaOption option = new DiaOption("OK");
            option.action = delegate
            {
                signalSended = true;

                Site.ForceReform(Site);
            };
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        private void GenerateItems()
        {
            ThingFilter filter = new ThingFilter();
            filter.SetAllow(ThingCategoryDefOf.ResourcesRaw, true);

            ThingSetMaker_MarketValue maker = new ThingSetMaker_MarketValue();
            ThingSetMakerParams parms = default;
            parms.totalMarketValueRange = new FloatRange(1600, 3000);
            parms.countRange = new IntRange(10, 25);
            parms.filter = filter;

            maker.fixedParams = parms;

            items = maker.Generate();
        }

        private List<Pawn> GenerateCarriers()
        {
            List<Pawn> carriers = new List<Pawn>();

            PawnKindDef pawnKindDef = allowedCarriers.RandomElement();
            int num = Rand.Range(2, 5);
            for(int i = 0; i < num; i++)
            {
                PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, Faction, PawnGenerationContext.NonPlayer);
                Pawn pawn = PawnGenerator.GeneratePawn(request);

                carriers.Add(pawn);
            }

            for(int i2 = 0; i2 < items.Count; i2++)
            {
                carriers.RandomElement().inventory.innerContainer.TryAdd(items[i2]);
            }
           
            return carriers;
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            base.EndQuest(caravan, condition);

            if(condition == EndCondition.None)
            {
                Find.LetterStack.ReceiveLetter("Quest_DarkNetCaptureMaterials_EndQuest_NoneTitle".Translate(), "Quest_DarkNetCaptureMaterials_EndQuest_NoneDesc".Translate(), LetterDefOf.NeutralEvent);
            }
        }

        public override void PostSiteRemove(QuestSite site)
        {
            base.PostSiteRemove(site);

            if (!signalSended)
                EndQuest(null, EndCondition.None);
            else
                EndQuest(null, EndCondition.Success);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToPirateCaravanMap caravanAction = new CaravanArrivalAction_EnterToPirateCaravanMap(mapParent);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref TraderDef, "TraderDef");
            Scribe_Collections.Look(ref items, "items", LookMode.Reference);
        }
    }
}
