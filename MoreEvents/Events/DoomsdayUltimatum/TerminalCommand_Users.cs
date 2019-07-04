using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class TerminalCommand_Users : TerminalCommand
    {
        public override string CommandKey => "/users";

        public override string CommandDescription => "DoomsdayTerminal_UsersCommand".Translate();

        private Doomsday_SecurityTerminal doomsdayTerminal;

        public override string Invoke()
        {
            doomsdayTerminal = Terminal as Doomsday_SecurityTerminal;
            StringBuilder builder = new StringBuilder();
            string[] commandParams = doomsdayTerminal.TerminalCommand.Split(' ');
            if (commandParams.Length > 2)
            {
               if (!string.IsNullOrEmpty(commandParams[2]) || (commandParams.Length == 4 && string.IsNullOrEmpty(commandParams[3])))
               {
                    if (doomsdayTerminal.HasAccess)
                    {
                        switch (commandParams[1])
                        {
                            case "add":
                                {
                                    doomsdayTerminal.Users.Add(commandParams[2], commandParams[3]);
                                    break;
                                }
                            case "delete":
                                {
                                    if (doomsdayTerminal.Users.ContainsKey(commandParams[2]))
                                    {
                                        string password = doomsdayTerminal.Users[commandParams[2]];
                                        if (commandParams[3] == password)
                                        {
                                            doomsdayTerminal.Users.Remove(commandParams[2]);
                                        }
                                        else
                                        {
                                            builder.AppendLine($"--> {"DoomsdayTerminal_WrongPassword".Translate()} < --");
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    DefaultCommand(builder, doomsdayTerminal);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        builder.AppendLine($"--> {"DoomsdayTerminal_WrongArguments".Translate()} < --");
                    }
                }
                else
                {
                    builder.AppendLine($"--> {"DoomsdayTerminal_NoAccess".Translate()} < --");
                }
            }
            else
            {
                DefaultCommand(builder, doomsdayTerminal);
            }

            if(commandParams.Length == 2 && commandParams[1] == "hack")
            {
                builder.AppendLine($"\n\n{"DoomsdayTerminal_UsersHack".Translate()}");
            }
            else
            {
                 builder.AppendLine($"\n{"DoomsdayTerminal_HackUsers".Translate()}");
            }

            return builder.ToString();
        }

        private void DefaultCommand(StringBuilder builder, Doomsday_SecurityTerminal doomsdayTerminal)
        {
            builder.Append($"{"DoomsdayTerminal_ActiveUsers".Translate()}\n");
            foreach (var user in doomsdayTerminal.Users)
            {
                builder.AppendLine($"- {user.Key}");
            }
            builder.AppendLine($"DoomsdayTerminal_AddNewUsers".Translate());
            builder.Append($"DoomsdayTerminal_RemoveUsers".Translate());
        }
    }
}
