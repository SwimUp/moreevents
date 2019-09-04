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
    }

    public class DarkNetProperties_Eisenberg : DarkNetProperties
    {
        public List<DrugSettings> DrugStockSettings;

        public DarkNetProperties_Eisenberg()
        {
            compClass = typeof(DarkNetComp_Eisenberg);
        }
    }
}
