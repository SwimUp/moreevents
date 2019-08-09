using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimOverhaul.Things
{
    public class Building_WorkTable_Animated : Building_WorkTable
    {
        public bool DisableAnimate;

        public bool Active { get
            {
                if (Map == null)
                    return false;


            } }
    }
}
