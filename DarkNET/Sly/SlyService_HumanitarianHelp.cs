using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkNET.Traders;
using RimWorld;
using Verse;

namespace DarkNET.Sly
{
    public class SlyService_HumanitarianHelp : SlyService_ItemsHelp
    {
        public override string Label => "SlyService_HumanitarianHelp_Label".Translate();

        public override string Description => "SlyService_HumanitarianHelp_Description".Translate(Cost_BaseHelp, Cost_AdvancedHelp, Cost_UltraHelp);

        private int Cost_BaseHelp => 300;

        private int Cost_AdvancedHelp => 600;

        private int Cost_UltraHelp => 1000;

        private Dictionary<ThingDef, int> baseHelpItems => new Dictionary<ThingDef, int>
        {
            {ThingDefOf.MealSimple, Rand.Range(10, 30) },
            {ThingDefOf.MedicineHerbal, Rand.Range(5, 15) }
        };

        private Dictionary<ThingDef, int> advancedHelpItems => new Dictionary<ThingDef, int>
        {
            {ThingDefOf.MealSurvivalPack, Rand.Range(15, 30) },
            {ThingDefOf.MedicineIndustrial, Rand.Range(5, 10) }
        };

        private Dictionary<ThingDef, int> ultraHelpItems => new Dictionary<ThingDef, int>
        {
            {ThingDefOf.MealSurvivalPack, Rand.Range(30, 50) },
            {ThingDefOf.MedicineUltratech, Rand.Range(5, 10) }
        };

        public override IEnumerable<FloatMenuOption> Options(Map map)
        {
            yield return new FloatMenuOption("SlyService_HumanitarianHelp_Base".Translate(), delegate
            {
                GenerateAndSendHelp(Cost_BaseHelp, map, baseHelpItems);
            });
            yield return new FloatMenuOption("SlyService_HumanitarianHelp_Advanced".Translate(), delegate
            {
                GenerateAndSendHelp(Cost_AdvancedHelp, map, advancedHelpItems);
            });
            yield return new FloatMenuOption("SlyService_HumanitarianHelp_Ultra".Translate(), delegate
            {
                GenerateAndSendHelp(Cost_UltraHelp, map, ultraHelpItems);
            });
        }
    }
}
