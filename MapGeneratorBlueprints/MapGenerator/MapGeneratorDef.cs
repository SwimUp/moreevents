﻿using MapGeneratorBlueprints.MapGenerator;
using RimWorld;
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
        public List<string> targetTags;

        public float Commonality;

        public IntVec2 size;

        public IntVec3 PawnsSpawnPos;

        public List<MapObject> MapData;

        public List<RoofObject> RoofData;
    }
}
