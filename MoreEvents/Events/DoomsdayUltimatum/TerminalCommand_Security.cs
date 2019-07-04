using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class TerminalCommand_Security : TerminalCommand
    {
        public override string CommandKey => "/security";

        public override string CommandDescription => "DoomsdatTerminal_SecurityCommand".Translate();

        public override string Invoke()
        {
            Doomsday_SecurityTerminal doomsdayTerminal = Terminal as Doomsday_SecurityTerminal;
            StringBuilder builder = new StringBuilder();
            if (doomsdayTerminal.HasAccess)
            {

            }
            else
            {
                builder.AppendLine($"--> {"DoomsdayTerminal_NoAccess".Translate()} < --");
            }

            return builder.ToString();
        }
    }
}
