using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Constellations.Conditions
{
    public class ConstellationCondition_Time : ConstellationCondition
    {
        public int MinHours;
        public int MaxHours;

        public override bool CanSpawn(Map affectedMap)
        {
            if (Current.Game.Maps.Count == 0)
                return false;

            Map map = Current.Game.Maps[0];
            if (GenLocalDate.HourInteger(map) >= MinHours && GenLocalDate.HourInteger(map) <= MaxHours)
                return true;

            return false;
        }
    }
}
