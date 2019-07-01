using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public class Condition_Gender : OptionCondition
    {
        public Gender AllowGender;
        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            if (p.gender != AllowGender)
                return false;

            return true;
        }

        public void PostLoad()
        {
            untranslatedId = $"{AllowGender}{ConditionId}";
        }
    }
}
