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

        public static string Translate(this AllianceRemoveReason reason)
        {
            switch (reason)
            {
                case AllianceRemoveReason.Kick:
                    return "AllianceRemoveReason_Kick".Translate();
                case AllianceRemoveReason.LowTrust:
                        return "AllianceRemoveReason_LowTrust".Translate();
                case AllianceRemoveReason.None:
                    return "AllianceRemoveReason_None".Translate();
                default:
                    return string.Empty;
            }
        }
    }
}
