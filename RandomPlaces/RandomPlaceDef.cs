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

        public MapGeneratorBlueprints.MapGenerator.MapGeneratorDef Map;
        public int MinAtStart = 0;
        public int MaxSpawn = -1;
        public FactionRelationKind FactionType;
        public ExtraLord ExtraLord;
        public CompRandomPlace Worker;
        public bool UseMapSpawnSpot;
        public bool RefuelGenerators = true;
        public SimpleCurve MinStartScale;
        public SimpleCurve MaxSpawnScale;
    }
}
