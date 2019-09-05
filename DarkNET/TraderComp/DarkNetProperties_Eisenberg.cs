using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static DarkNET.Traders.TraderWorker_Eisenberg;

namespace DarkNET.TraderComp
{
    public class DrugSettings
    {
        public Tab Tab;

        public IntRange CountRange;

        public FloatRange ValueRange;

        public List<DarkNetGood> Goods;

        public void ResolveReferences()
        {
            foreach(var good in Goods)
            {
                good.ThingFilter.ResolveReferences();
            }
        }
    }

    public class DarkNetProperties_Eisenberg : DarkNetProperties
    {
        public List<DrugSettings> DrugStockSettings;

        public DarkNetProperties_Eisenberg()
        {
            compClass = typeof(DarkNetComp_Eisenberg);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            foreach(var setting in DrugStockSettings)
            {
                setting.ResolveReferences();
            }
        }
    }
}
