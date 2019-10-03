using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class DarkNetProperties_JohnCarver : DarkNetProperties
    {
        public List<CategoryItemSetting<TraderWorker_JohnCarver.Tab>> CategoryItemSettings;

        public DarkNetProperties_JohnCarver()
        {
            compClass = typeof(DarkNetComp_JohnCarver);
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
