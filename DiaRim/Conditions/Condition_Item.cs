using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public class Condition_Item : OptionCondition
    {
        public ThingDef ThingDef;

        public int Count;

        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            if (ThingDef == null)
                return true;

            if (Count == 0)
                return true;

            int resourceCount = p.Map.resourceCounter.GetCount(ThingDef);

            if (resourceCount < Count)
                return false;

            return true;
        }
    }
}
