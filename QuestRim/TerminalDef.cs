using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class TerminalDef : Def
    {
        public string InitialText;

        public List<TerminalCommand> Commands;
    }
}
