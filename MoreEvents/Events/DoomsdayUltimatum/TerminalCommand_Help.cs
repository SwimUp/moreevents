using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class TerminalCommand_Help : TerminalCommand
    {
        public override string CommandKey => "/help";

        public override string Invoke()
        {
            //StringBuilder builder = new StringBuilder();
            //builder.AppendLine("--> ");

            //return builder.ToString();
            return "";
        }
    }
}
