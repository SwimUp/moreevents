using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.AI
{
    public class PawnsArrivalModeWorker_Surroundings : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < pawns.Count; i++)
            {
                if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(map) && c.Walkable(map) && c.Standable(map), map, 0f, out IntVec3 pos))
                {
                    GenSpawn.Spawn(pawns[i], pos, map, parms.spawnRotation);
                }
            }
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            return true;
        }
    }
}
