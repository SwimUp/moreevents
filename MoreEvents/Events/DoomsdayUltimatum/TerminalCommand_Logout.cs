using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class TerminalCommand_Logout : TerminalCommand
    {
        public override string CommandKey => "/logout";

        public override string CommandDescription => "DoomsdayTerminal_LogoutCommand".Translate();

        public override string Invoke()
        {
            Doomsday_SecurityTerminal doomsdayTerminal = Terminal as Doomsday_SecurityTerminal;
            StringBuilder builder = new StringBuilder();

            if (string.IsNullOrEmpty(doomsdayTerminal.CurrentUser) || doomsdayTerminal.CurrentUser == "Anonymous")
            {
                builder.AppendLine($"{"DoomsdayTerminal_LoginFirst".Translate()}");
            }
            else
            {
                doomsdayTerminal.CurrentUser = "Anonymous";
                doomsdayTerminal.HasAccess = false;
                builder.AppendLine($"{"DoomsdayTerminal_LogoutOk".Translate()}");
            }

            return builder.ToString();
        }
    }
}
