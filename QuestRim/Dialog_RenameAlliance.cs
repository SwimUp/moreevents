using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class Dialog_RenameAlliance : Dialog_Rename
    {
        private Alliance alliance;

        public Dialog_RenameAlliance(Alliance alliance)
        {
            this.alliance = alliance;
        }
        protected override void SetName(string name)
        {
            alliance.Name = name;
        }
    }
}
