using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace TestTools.MapGenerator
{
    public class MapObject : IExposable
    {
        public ThingData key;

        public List<IntVec3> value;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref key, "key");
            Scribe_Collections.Look(ref value, "value", LookMode.Value);
        }
    }

    public class ThingData : IExposable
    {
        public ThingDef Thing;
        public ThingDef Stuff;
        public QualityCategory Quality;
        public TerrainDef Terrain;
        public int Count;
        public PawnKindDef Kind;
        public FactionDef Faction = null;
        public Rot4 Rotate;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Thing, "Thing");
            Scribe_Defs.Look(ref Stuff, "Stuff");
            Scribe_Values.Look(ref Quality, "Quality");
            Scribe_Defs.Look(ref Terrain, "Terrain");
            Scribe_Values.Look(ref Count, "Count");
            Scribe_Defs.Look(ref Kind, "Kind");
            Scribe_Defs.Look(ref Faction, "Faction");
            Scribe_Values.Look(ref Rotate, "Rotate");
        }
    }
}
