using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public abstract class TerminalCommand
    {
        public abstract string CommandKey { get; }

        public Terminal Terminal;

        public abstract string Invoke();
    }
}
