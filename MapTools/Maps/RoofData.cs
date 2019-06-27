using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace TestTools.MapGenerator
{
    public class RoofObject : IExposable
    {
        public RoofDef RoofDef;

        public List<IntVec3> Positions;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref RoofDef, "RoofDef");
            Scribe_Collections.Look(ref Positions, "Positions", LookMode.Value);
        }
    }
}
