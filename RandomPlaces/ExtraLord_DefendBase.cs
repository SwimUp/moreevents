using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RandomPlaces
{
    public class ExtraLord_DefendBase : ExtraLord
    {
        public IntVec3 BaseCenter;
        public override Lord GetLord(Faction faction, Map map)
        {
            LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(faction, BaseCenter);
            return LordMaker.MakeNewLord(faction, lordJob_DefendBase, map);
        }
    }
}
