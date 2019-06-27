using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace TestTools.MapGenerator
{
    public class MapGeneratorData : IExposable
    {
        public string defName;

        public IntVec2 size;

        public List<MapObject> MapData;

        public List<RoofObject> RoofData;

        public void ExposeData()
        {
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref size, "size");
            Scribe_Collections.Look(ref MapData, "MapData", LookMode.Deep);
            Scribe_Collections.Look(ref RoofData, "RoofData", LookMode.Deep);
        }
    }
}
