using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.TraderComp
{
    public class DarkNetProperties_RogerEdmonson : DarkNetProperties
    {
        public List<OrderItem> OrderBodyparts;

        public ThingFilter specialGoodsFilter;

        public DarkNetProperties_RogerEdmonson()
        {
            compClass = typeof(DarkNetComp_RogerEdmonson);
        }

        public override void ResolveReferences()
        {
            specialGoodsFilter.ResolveReferences();
        }
    }
}
