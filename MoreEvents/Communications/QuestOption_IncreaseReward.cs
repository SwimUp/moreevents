using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class QuestOption_IncreaseReward : QuestOption
    {
        public override string Label => "QuestOption_IncreaseReward_Label".Translate(SkillLevel, SkillDef.LabelCap);

        public int SkillLevel;
        public SkillDef SkillDef;
        public FloatRange NewValueRange;
        public IntRange NewCountRange;

        public QuestOption_IncreaseReward()
        {

        }

        public QuestOption_IncreaseReward(int skillLevel, SkillDef skillDef, FloatRange newValueRange, IntRange newCountRange)
        {
            SkillLevel = skillLevel;
            SkillDef = skillDef;
            NewValueRange = newValueRange;
            NewCountRange = newCountRange;
        }

        public override void DoAction(QuestRim.Quest quest, Pawn speaker, Pawn defendant)
        {
            SkillRecord skill = speaker.skills.GetSkill(SkillDef);
            if (skill.Level < SkillLevel)
            {
                Messages.Message("QuestOption_IncreaseReward_Level".Translate(SkillDef.LabelCap, SkillLevel, skill.Level), MessageTypeDefOf.NeutralEvent, false);
                return;
            }

            quest.GenerateRewards(quest.GetQuestThingFilter(), NewValueRange, NewCountRange, null, null);

            quest.Options.Remove(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref SkillLevel, "SkillLevel");
            Scribe_Values.Look(ref NewValueRange, "NewValueRange");
            Scribe_Values.Look(ref NewCountRange, "NewCountRange");
            Scribe_Defs.Look(ref SkillDef, "SkillDef");
        }
    }
}
