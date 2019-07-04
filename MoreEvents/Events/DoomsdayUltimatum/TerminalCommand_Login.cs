using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class TerminalCommand_Login : TerminalCommand
    {
        public override string CommandKey => "/login";

        public override string CommandDescription => "DoomsdayTerminal_LoginCommand".Translate();   

        public override string Invoke()
        {
            Doomsday_SecurityTerminal doomsdayTerminal = Terminal as Doomsday_SecurityTerminal;
            StringBuilder builder = new StringBuilder();

            if (string.IsNullOrEmpty(doomsdayTerminal.CurrentUser) || doomsdayTerminal.CurrentUser == "Anonymous")
            {
                builder.AppendLine($"{"DoomsdayTerminal_AlreadyLogged".Translate()}");
            }
            else
            {
                string[] commandParams = doomsdayTerminal.TerminalCommand.Split(' ');
                if (commandParams.Length == 3)
                {
                    if (doomsdayTerminal.Users.ContainsKey(commandParams[1]))
                    {
                        string user = commandParams[1];
                        string password = doomsdayTerminal.Users[user];
                        if (commandParams[2] == password)
                        {
                            doomsdayTerminal.CurrentUser = user;
                            doomsdayTerminal.HasAccess = true;
                            builder.AppendLine($"{"DoomsdayTerminal_LogginSuccess".Translate(user)}");
                        }
                        else
                        {
                            builder.AppendLine($"{"DoomsdayTerminal_LogginFail".Translate()}");
                        }
                    }
                    else
                    {
                        builder.AppendLine($"{"DoomsdayTerminal_LogginFail".Translate()}");
                    }
                }
                else
                {
                    builder.AppendLine($"--> {"DoomsdayTerminal_WrongArguments".Translate()} < --");
                }
            }

            return builder.ToString();
        }
    }
}
