using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public class Condition_Trait : OptionCondition
    {
        public TraitDef ReqTrait;
        public int Degree;
        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            if (p == null)
                return true;

            var trait = p.story.traits.GetTrait(ReqTrait);

            if (trait != null && trait.Degree == Degree)
                return true;

            return false;
        }

        public void PostLoad()
        {
            untranslatedId = $"traitReq{ConditionId}";
        }
    }
}
