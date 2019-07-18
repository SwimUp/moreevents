using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events
{
    public class IncidentWorker_SeaAir : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["SeaAir"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            Rot4 rot = Find.World.CoastDirectionAt(map.Tile);
            if (!rot.IsValid)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;

            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (!pawn.Dead && pawn.RaceProps.Humanlike && pawn.RaceProps.IsFlesh)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.SeaAir);
                }
            }

            SendStandardLetter();

            return true;
        }
    }
}
