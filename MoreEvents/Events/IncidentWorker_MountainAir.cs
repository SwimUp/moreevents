using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_MountainAir : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MountainAir"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            if (map.TileInfo.hilliness != Hilliness.LargeHills && map.TileInfo.hilliness != Hilliness.Mountainous)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            if (map.mapPawns == null)
                return false;

            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn != null && !pawn.Dead && pawn.RaceProps.Humanlike && pawn.RaceProps.IsFlesh)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.MountainAir);
                }
            }

            SendStandardLetter(parms, null);

            return true;
        }
    }
}
