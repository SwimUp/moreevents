using MoreEvents.Things;
using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class Doomsday_SecurityTerminal : Terminal
    {
        public Building_DoomsdayGun weapon;

        public Doomsday_SecurityTerminal(TerminalDef terminalDef, Building_DoomsdayGun weapon) : base(terminalDef)
        {
            this.weapon = weapon;
        }
    }
}
