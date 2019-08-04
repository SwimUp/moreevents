using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class Apparel_Mk1 : Apparel_MkArmor
    {
        public override Apparel GetHelmet => Wearer.apparel.WornApparel.Where(a => a.def == RimArmorCore.ThingDefOfLocal.Apparel_MK1ThunderHead).FirstOrDefault();

        public override int[] SlotsNumber => new int[]
        {
            1,
            1,
            2,
            1
        };
    }
}
