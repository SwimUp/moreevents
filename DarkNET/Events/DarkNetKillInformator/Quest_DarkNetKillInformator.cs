using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using QuestRim;
using RimOverhaul.Gss;
using RimOverhaul.Quests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Events.DarkNetKillInformator
{
    public class Quest_DarkNetKillInformator : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;
        public override string CardLabel => string.Format(IncidentDefOfLocal.DarkNetKillInformator.letterLabel, TraderDef.LabelCap, Faction.Name);

        public override string Description => string.Format(IncidentDefOfLocal.DarkNetKillInformator.letterText, TraderDef.LabelCap, Faction.Name);

        public override string PlaceLabel => "Quest_DarkNetKillInformator_PlaceLabel".Translate();

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

        public override string ExpandingIconPath => "Quests/Quest_DarkNetKillInformator";

        private DarkNetTrader darkNetTrader;

        public DarkNetTraderDef TraderDef;

        private string mapGeneratTag => "Quest_DarkNetKillInformator";

        private MapGeneratorBlueprints.MapGenerator.MapGeneratorDef mapGeneratorDef;

        public override bool HasExitCells => true;

        public override bool UseLeaveCommand => false;

        public Pawn Informator;

        public bool Visited;

        public Quest_DarkNetKillInformator() : base()
        {

        }

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            MapGeneratorHandler.GenerateMap(mapGeneratorDef, map, out List<Pawn> pawns, false, true, true, false, true, true, true, GssRaids.GssFaction);

            Informator = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier);
            GenSpawn.Spawn(Informator, mapGeneratorDef.PawnsSpawnPos, map);

            Visited = true;

            UnlimitedTime = true;

            ShowInfo();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToQuestMapWithGenerator caravanAction = new CaravanArrivalAction_EnterToQuestMapWithGenerator(mapParent, mapGeneratorDef);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanAction, "EnterToMapQuest_Option".Translate(), caravan, mapParent.Tile, mapParent);
        }

        private void ShowInfo()
        {
            DiaNode node = new DiaNode("Quest_DarkNetKillInformator_Info".Translate(Informator.Name.ToStringFull));
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
        }

        public Quest_DarkNetKillInformator(DarkNetTraderDef traderDef)
        {
            TraderDef = traderDef;

            DefDatabase<MapGeneratorBlueprints.MapGenerator.MapGeneratorDef>.AllDefsListForReading.Where(gen => gen.targetTags != null && gen.targetTags.Contains(mapGeneratTag)).TryRandomElementByWeight(w => w.Commonality, out mapGeneratorDef);
        }

        public override void PostSiteRemove(QuestSite site)
        {
            base.PostSiteRemove(site);

            if (Visited && (Informator == null || Informator.Dead))
                EndQuest(null, EndCondition.Success);
            else
                EndQuest(null, EndCondition.Fail);
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            if(condition == EndCondition.Fail || condition == EndCondition.None)
            {
                if(Rand.Chance(0.35f))
                {
                    Current.Game.GetComponent<DarkNet>().SendGssRaid(Find.AnyPlayerHomeMap, true);
                }
                else
                {
                    Find.LetterStack.ReceiveLetter("Quest_DarkNetKillInformator_InformatorDeadTitle".Translate(), "Quest_DarkNetKillInformator_InformatorDeadDesc".Translate(), LetterDefOf.NeutralEvent);
                }
            }

            base.EndQuest(caravan, condition);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref TraderDef, "TraderDef");
            Scribe_Defs.Look(ref mapGeneratorDef, "mapGeneratorDef");
            Scribe_References.Look(ref Informator, "Informator");
            Scribe_Values.Look(ref Visited, "Visited");
        }
    }
}
