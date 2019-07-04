using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public abstract class TerminalCommand
    {
        public abstract string CommandKey { get; }
        public abstract string CommandDescription { get; }

        public bool ShowInHelp = true;

        public Terminal Terminal;

        public abstract string Invoke();
    }
}
