using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Constellations.Conditions
{
    public class ConstellationCondition_PawnCount : ConstellationCondition
    {
        public int MinColonists;

        public override bool CanSpawn(Map affectedMap)
        {
            if (Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction == Faction.OfPlayer).Count() >= MinColonists)
                return true;

            return false;
        }
    }
}
