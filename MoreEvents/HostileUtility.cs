using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul
{
    public static class HostileUtility
    {
        public static bool AnyNonDeadHostileOnMap(Map map, Faction enemyFaction)
        {
            List<Pawn> enemyPawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == enemyFaction && !p.Dead && p.RaceProps.Humanlike).ToList();

            if (enemyPawns == null || enemyPawns.Count == 0)
                return false;

            return true;
        }

        public static bool AnyNonDownedHostileOnMap(Map map, Faction enemyFaction)
        {
            List<Pawn> enemyPawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == enemyFaction && p.health.State == PawnHealthState.Mobile && p.RaceProps.Humanlike).ToList();

            if (enemyPawns == null || enemyPawns.Count == 0)
                return false;

            return true;
        }
    }
}
