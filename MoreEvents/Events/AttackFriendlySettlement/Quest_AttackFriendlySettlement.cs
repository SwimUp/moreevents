using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class Quest_AttackFriendlySettlement : QuestRim.Quest
    {
        public override string CardLabel => "Quest_AttackFriendlySettlement_CardLabel".Translate();

        public override int SuccessTrustAffect => 15;

        public override int FailTrustAffect => -15;

        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;
        public override string Description => "Quest_AttackFriendlySettlement_Description".Translate(Faction.leader.Name.ToStringFull, Faction.Name);

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            base.EndQuest(caravan, condition);

            if(condition == EndCondition.Success)
            {
                ScoutingComp.GiveScoutingComp(Faction, 2, 15, 5, 5);
            }
        }
    }
}
