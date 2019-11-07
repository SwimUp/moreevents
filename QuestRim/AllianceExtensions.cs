using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public static class AllianceExtensions
    {
        public static bool InAnyAlliance(this FactionInteraction interaction)
        {
            return QuestsManager.Communications.FactionManager.Alliances.Any(x => x.Factions.Contains(interaction));
        }

        public static bool InAlliance(this FactionInteraction interaction, Alliance alliance)
        {
            return alliance.Factions.Contains(interaction);
        }
    }
}
