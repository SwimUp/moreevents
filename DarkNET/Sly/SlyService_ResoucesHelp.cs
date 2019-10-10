using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Sly
{
    public class SlyService_ResoucesHelp : SlyService_ItemsHelp
    {
        public override string Label => "SlyService_ResoucesHelp_Label".Translate();

        public override string Description => "SlyService_ResoucesHelp_Description".Translate(baseMaterials, advancedMaterials, ultraMaterials);

        private int baseMaterials => 200;

        private int advancedMaterials => 500;

        private int ultraMaterials => 1000;

        private FloatRange baseMaterialsPriceRange => new FloatRange(200, 400);
        private IntRange baseMaterialsCountRange => new IntRange(30, 70);
        private List<ThingDef> baseMaterialsThings => new List<ThingDef>
        {
            ThingDefOfLocal.BlocksSandstone,
            ThingDefOfLocal.BlocksGranite,
            ThingDefOfLocal. BlocksLimestone,
            ThingDefOfLocal. BlocksSlate,
            ThingDefOfLocal.BlocksMarble,
            ThingDefOf.WoodLog
        };

        private FloatRange advancedMaterialsPriceRange => new FloatRange(400, 700);
        private IntRange advancedMaterialsCountRange => new IntRange(20, 50);
        private List<ThingDef> advancedMaterialsThings => new List<ThingDef>
        {
            MoreEvents.ThingDefOfLocal.IronOre,
            MoreEvents.ThingDefOfLocal.CopperOre,
            MoreEvents.ThingDefOfLocal.Coal
        };

        private FloatRange ultraMaterialsPriceRange => new FloatRange(800, 1400);
        private IntRange ultraMaterialsCountRange => new IntRange(10, 20);
        private List<ThingDef> ultraMaterialsThings => new List<ThingDef>
        {
            MoreEvents.ThingDefOfLocal.NickelOre,
            MoreEvents.ThingDefOfLocal.IlmeniteOre,
            MoreEvents.ThingDefOfLocal.Chromium
        };

        public override IEnumerable<FloatMenuOption> Options(Map map)
        {
            yield return new FloatMenuOption("SlyService_ResoucesHelp_Base".Translate(), delegate
            {
                GenerateAndSendHelp(baseMaterials, map, baseMaterialsPriceRange, baseMaterialsCountRange, baseMaterialsThings);
            });
            yield return new FloatMenuOption("SlyService_ResoucesHelp_Advanced".Translate(), delegate
            {
                GenerateAndSendHelp(advancedMaterials, map, advancedMaterialsPriceRange, advancedMaterialsCountRange, advancedMaterialsThings);
            });
            yield return new FloatMenuOption("SlyService_ResoucesHelp_Ultra".Translate(), delegate
            {
                GenerateAndSendHelp(ultraMaterials, map, ultraMaterialsPriceRange, ultraMaterialsCountRange, ultraMaterialsThings);
            });
        }
    }
}
