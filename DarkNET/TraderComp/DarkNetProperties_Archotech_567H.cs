using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class DarkNetProperties_Archotech_567H : DarkNetProperties
    {
        public List<CategoryItemSetting<TraderWorker_Archotech_567H.Tab>> CategoryItemSettings;

        public bool enableQuests = true;

        public DarkNetProperties_Archotech_567H()
        {
            compClass = typeof(DarkNetComp_Archotech_567H);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            foreach (var setting in CategoryItemSettings)
            {
                setting.ResolveReferences();
            }
        }
    }
}
