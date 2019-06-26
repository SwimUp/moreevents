﻿using MapGeneratorBlueprints.MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Verse;

namespace MapGeneratorBlueprints.MapGenerator
{
    public class MapGeneratorDef : Def
    {
        public List<MapObject> MapData;

        public List<RoofObject> RoofData;

        public IntVec2 size;
    }
}
