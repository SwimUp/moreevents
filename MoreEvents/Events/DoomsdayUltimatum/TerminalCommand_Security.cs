using QuestRim;
using RimWorld;
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

        private string[] words = new string[]
        {
            "COATE",
            "SERMON",
            "STEO",
            "LLEN@",
            "SHINE",
            "EDEM",
            "WANTED",
            "BETTER",
            "WRITER"
        };
        private int maxWords => words.Length;
        private int currentWords = 0;

        public override string CommandDescription => "DoomsdatTerminal_SecurityCommand".Translate();

        public override string Invoke()
        {
            Doomsday_SecurityTerminal doomsdayTerminal = Terminal as Doomsday_SecurityTerminal;
            StringBuilder builder = new StringBuilder();
            if (doomsdayTerminal.HasAccess)
            {
                string[] commandParams = doomsdayTerminal.TerminalCommand.Split(' ');
                if(commandParams.Length >= 2)
                {
                    switch (commandParams[1])
                    {
                        case "dump":
                            {
                                builder.AppendLine($"{"DoomsdayTerminal_HackToolsSecurity".Translate()}");
                                GenerateCode(builder);
                                break;
                            }
                        case "disable":
                            {
                                if (doomsdayTerminal.SecutiyAccess && doomsdayTerminal.Power)
                                {
                                    doomsdayTerminal.weapon.SecuritySystemActive = false;
                                    Find.LetterStack.ReceiveLetter("SecurityDoomsdayOffTitle".Translate(), "SecurityDoomsdayOff".Translate(), LetterDefOf.PositiveEvent);
                                    builder.AppendLine($"{"DoomsdayTerminal_SecurityDisabled".Translate()}");
                                }
                                else
                                {
                                    builder.AppendLine($"{"DoomsdayTerminal_SecurityNoPower".Translate()}");
                                }

                               break;
                            }
                        case "energy":
                            {
                                if(doomsdayTerminal.Power)
                                {
                                    builder.AppendLine($"{"DoomsdayTerminal_SecurityPowerOk".Translate()}");
                                }
                                else if(doomsdayTerminal.SecutiyAccess)
                                {
                                    if (!doomsdayTerminal.ActiveMode)
                                    {
                                        builder.AppendLine($"{"DoomsdayTerminal_SecurityPowerInfoPassive".Translate()}");
                                    }
                                    else
                                    {
                                        builder.AppendLine($"{"DoomsdayTerminal_SecurityPowerInfoActive".Translate()}");
                                    }

                                    if(commandParams.Length == 3)
                                    {
                                        switch(commandParams[2])
                                        {
                                            case "activemode":
                                                {
                                                    builder.AppendLine($"{"DoomsdayTerminal_ActiveModeEnable".Translate()}");
                                                    doomsdayTerminal.ActiveMode = true;
                                                    break;
                                                }
                                            case "shields":
                                                {
                                                    if (!doomsdayTerminal.ActiveMode)
                                                    {
                                                        builder.AppendLine($"{"DoomsdayTerminal_ActiveModeReq".Translate()}");
                                                    }
                                                    else
                                                    {
                                                        builder.AppendLine($"{"DoomsdayTerminal_ShieldsOff".Translate()}");
                                                    }

                                                    break;
                                                }
                                            case "on":
                                                {
                                                    if (!doomsdayTerminal.ActiveMode)
                                                    {
                                                        builder.AppendLine($"{"DoomsdayTerminal_ActiveModeReq".Translate()}");
                                                    }
                                                    else
                                                    {
                                                        if(!doomsdayTerminal.Power)
                                                        {
                                                            builder.AppendLine($"{"DoomsdayTerminal_PowerActivating".Translate()}");
                                                            doomsdayTerminal.Power = true;
                                                        }

                                                        builder.AppendLine($"{"DoomsdayTerminal_SecurityPowerOk".Translate()}");
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }
                                else
                                {
                                    builder.AppendLine($"--> {"DoomsdayTerminal_NoAccess".Translate()} < --");
                                }

                                break;
                            }
                        default:
                            {
                                if (!doomsdayTerminal.SecutiyAccess)
                                {
                                    if (commandParams[1] == doomsdayTerminal.SecurityPassword)
                                    {
                                        doomsdayTerminal.SecutiyAccess = true;
                                        builder.AppendLine($"{"DoomsdayTerminal_SecurityAccessGranted".Translate()}");
                                    }
                                    else
                                    {
                                        builder.AppendLine($"{"DoomsdayTerminal_WrongPassword".Translate()}");
                                    }
                                }

                                break;
                            }
                    }
                }
                else
                {
                    if (!doomsdayTerminal.SecutiyAccess)
                        builder.AppendLine($"{"DoomsdayTerminal_SecurityFirst".Translate()}");
                    else
                        builder.AppendLine($"{"DoomsdayTerminal_SecurityAccessGranted".Translate()}");
                }
            }
            else
            {
                builder.AppendLine($"--> {"DoomsdayTerminal_NoAccess".Translate()} < --");
            }

            return builder.ToString();
        }

        private void GenerateCode(StringBuilder builder)
        {
            currentWords = 0;

            for (int i = 0; i < 10; i++)
            {
                builder.AppendLine($"0xF{GenerateString()}  {GenerateString(10, true)}   \t\t0xF{GenerateString()}  {GenerateString(10, true)}");
            }
        }

        public string GenerateString(int length = 3, bool withExtra = false)
        {
            char[] chars = null;
            if (!withExtra)
                chars = new char[] { 'Q', '7', '4', 'H', 'T', '7', 'F', '1', '2', '3', '4', '5', 'I', '1', '2', 'C', 'F', '8', 'A', 'C', '2', 'G', 'F', '6' };
            else
                chars = new char[] { 'Q', 'W', 'E', '!', '@', 'F', 'i', '1', '2', '$', '4', '*', '&', ')', '(', 'H', 'C', 'M', 'A', '*', '№', '"', '!', 'j' };

            StringBuilder builder = new StringBuilder(length);

            bool worldAppend = false;
            for (int i = 0; i < length; i++)
            {
                if (currentWords < maxWords && !worldAppend && (length - words[currentWords].Length) > i)
                {
                    if (Rand.Chance(0.23f))
                    {
                        i += words[currentWords].Length;
                        builder.Append(words[currentWords]);
                        currentWords++;
                        worldAppend = true;
                    }
                    else
                    {
                        builder.Append(chars[Rand.Range(0, chars.Length)]);
                    }
                }
                else
                {
                    builder.Append(chars[Rand.Range(0, chars.Length)]);
                }
            }

            return builder.ToString();
        }
    }
}
