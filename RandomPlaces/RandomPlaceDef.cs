using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public class RandomPlaceDef : Def
    {
        public float Commonality;

        public List<MapData> Maps;
    }

    public class MapData
    {
        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef Map;
        public int MinAtStart = 0;
        public int MaxSpawn = -1;
        public float Commonality = 1f;
        public FactionRelationKind FactionType;
        public Type Worker;
    }
}
