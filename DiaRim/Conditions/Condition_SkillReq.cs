using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public class Condition_SkillReq : OptionCondition
    {
        public SkillDef Skill;
        public int SkillLevel;

        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            var skill = p.skills.GetSkill(Skill);

            if (skill.TotallyDisabled)
                return false;

            if (skill.Level < SkillLevel)
                return false;

            return true;
        }

        public void PostLoad()
        {
            untranslatedId = $"skillReq{ConditionId}";
        }
    }
}
