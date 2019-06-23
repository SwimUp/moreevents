using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public class Condition_Backstory : OptionCondition
    {
        public string Childhood;

        public string Adulthood;

        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            if (!string.IsNullOrEmpty(Childhood))
            {
                if (p.story.childhood.identifier != Childhood)
                    return false;
            }

            if (!string.IsNullOrEmpty(Adulthood))
            {
                if (p.story.adulthood.identifier != Adulthood)
                    return false;
            }

            return true;
        }
    }
}
