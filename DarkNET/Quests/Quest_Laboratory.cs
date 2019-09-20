using DarkNET.Caravans;
using MapGeneratorBlueprints.MapGenerator;
using MoreEvents;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Quests
{
    public class Quest_Laboratory : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => "Quest_Laboratory_CardLabel".Translate();

        public override string Description => "Quest_Laboratory_Description".Translate();

        public override string PlaceLabel => "Quest_Laboratory_PlaceLabel".Translate();

        public override string ExpandingIconPath => "Quests/Quest_Laboratory";

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
            foreach(var p in pawns)
            {
                if(p.RaceProps.Animal)
                {
                    p.SetFaction(mutantsFaction);
                }
            }

            DiaNode node = new DiaNode("Quest_Laboratory_Info1".Translate());
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);

            UnlimitedTime = true;
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
