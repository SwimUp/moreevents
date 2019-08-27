using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Traders
{
    public class OrderItem : IExposable
    {
        public OrderBodypartGroup BodypartGroup;

        public ThingDef ThingDef;

        public float Commonality;

        public void ExposeData()
        {
            Scribe_Values.Look(ref BodypartGroup, "Group");
            Scribe_Defs.Look(ref ThingDef, "ThingDef");
            Scribe_Values.Look(ref Commonality, "Commonality");
        }
    }
}
