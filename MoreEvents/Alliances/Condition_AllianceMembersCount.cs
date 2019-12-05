using DiaRim;
using DiaRim.Conditions;
using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Alliances
{
    public class Condition_AllianceMembersCount : OptionCondition
    {
        public int MinMembers;

        public override bool CanUse(Pawn p, DialogOption option = null)
        {
            FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(p.Faction);
            if (interaction != null)
            {
                Alliance alliance = interaction.Alliance;
                if (alliance != null)
                {
                    if (alliance.FactionOwner == interaction.Faction)
                    {
                        if (alliance.Factions.Count >= MinMembers)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
