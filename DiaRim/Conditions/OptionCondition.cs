using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public abstract class OptionCondition
    {
        public string DisableReason;

        public abstract bool CanUse(Pawn p, DialogOption option = null);
    }
}
