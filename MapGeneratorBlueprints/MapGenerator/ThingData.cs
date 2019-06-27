using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MapGeneratorBlueprints.MapGenerator
{
    public class ThingData
    {
        public ThingDef Thing;
        public ThingDef Stuff;
        public QualityCategory Quality;
        public TerrainDef Terrain;
        public int Count;
        public PawnKindDef Kind;
        public FactionDef Faction = null;
        public Rot4 Rotate;
    }
}
