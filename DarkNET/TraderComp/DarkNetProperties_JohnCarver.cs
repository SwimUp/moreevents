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
        public class CategoryItemSetting
        {
            public TraderWorker_JohnCarver.Tab Tab;

            public IntRange CountRange;

            public FloatRange ValueRange;

            public List<DarkNetGood> Goods;

            public float PriceMultiplier = 1f;

            public void ResolveReferences()
            {
                foreach (var good in Goods)
                {
                    good.ThingFilter.ResolveReferences();
                }
            }
        }

        public List<CategoryItemSetting> CategoryItemSettings;

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
