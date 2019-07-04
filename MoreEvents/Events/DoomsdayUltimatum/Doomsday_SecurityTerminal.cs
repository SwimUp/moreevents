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

        public bool HasAccess = false;

        public Dictionary<string, string> Users = new Dictionary<string, string>
        {
            {"Fernal", "DoomsdayTerminal_UsersHackAnser".Translate() },
            {"AktunOba", "77822233" },
            {"Twister", "99802233" },
            {"Dimon228", "5615633" }
        };

        public string CurrentUser = "Anonymous";

        public Doomsday_SecurityTerminal(TerminalDef terminalDef, Building_DoomsdayGun weapon) : base(terminalDef)
        {
            this.weapon = weapon;
        }
    }
}
