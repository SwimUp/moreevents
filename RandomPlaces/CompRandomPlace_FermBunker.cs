using MoreEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public class CompRandomPlace_FermBunker : CompRandomPlace
    {
        public override void PostMapGenerate(Map map, List<Pawn> pawns)
        {
            IntVec3 lukePos = new IntVec3(104, 0, 73);
            ThingDef def = ThingDefOfLocal.Trigger_Luke;

            Building thing = (Building)GenSpawn.Spawn(def, lukePos, map);
        }
    }
}
