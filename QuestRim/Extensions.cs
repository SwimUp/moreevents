using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestRim
{
    public static class Extensions
    {
        public static bool Contains(this List<FactionInteraction> list, Faction faction)
        {
            foreach(var inter in list)
            {
                if(inter.Faction == faction)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
