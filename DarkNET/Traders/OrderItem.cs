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

        public List<OrderThing> Items;

        public void ExposeData()
        {
            Scribe_Values.Look(ref BodypartGroup, "Group");
            Scribe_Collections.Look(ref Items, "Items", LookMode.Deep);
        }
    }

    public class OrderThing : IExposable
    {
        public ThingDef ThingDef;

        public float Commonality;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref ThingDef, "ThingDef");
            Scribe_Values.Look(ref Commonality, "Commonality");
        }
    }
}
