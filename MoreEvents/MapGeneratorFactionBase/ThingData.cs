using MapGenerator;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.MapGeneratorFactionBase
{
    public class ThingData
    {
        public ThingDef Thing;
        public Dictionary<ThingDef, float> Stuff = new Dictionary<ThingDef, float>();
        public Dictionary<QualityCategory, float> Quality = new Dictionary<QualityCategory, float>();
        public float Chance = 1f;
        public TerrainDef Terrain;
        public IntRange Count;
        public IntRange HealthRange;
        public PawnKindDef Kind;
        public FactionDef Faction = null;
        public LordType LordType = LordType.Defend;
    }
}
