using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class CompTest : ThingComp
    {
        public override void CompTick()
        {
            base.CompTick();

            Log.Message("213");
        }
    }
}
