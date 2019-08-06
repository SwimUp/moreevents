using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class Apparel_Mk2 : Apparel_MkArmor
    {
        public override Apparel GetHelmet => Wearer.apparel.WornApparel.Where(a => a.def == RimArmorCore.ThingDefOfLocal.Apparel_MK2AquilleHead).FirstOrDefault();

        public override int[] SlotsNumber => new int[]
        {
            2,
            2,
            1,
            1
        };
    }
}
