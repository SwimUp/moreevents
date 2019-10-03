using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class CategoryItemSetting<T>
    {
        public T Tab;

        public IntRange CountRange;

        public FloatRange ValueRange;

        public QualityRange QualityRange;

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
}
