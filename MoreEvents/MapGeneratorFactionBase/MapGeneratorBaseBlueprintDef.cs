using MoreEvents.MapGeneratorFactionBase;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MapGenerator
{
    public class MapGeneratorBaseBlueprintDef : Def
    {
        public bool mapCenterBlueprint = true;

        public string createdBy;

        //public TechLevel techLevelRequired = TechLevel.Undefined;
        //public TechLevel techLevelMax = TechLevel.Undefined;
        //public float chance = 1;

        public IntVec2 size;
        public ThingDef defaultBuildingMaterial = null;
        public string buildingData;
        public string nonbuildingData;
        public string floorData;
        public string pawnData;
        public string itemData;
        public bool canHaveHoles;
        public bool createTrigger;
        public string TriggerLetterLabel = null;
        public string TriggerLetterMessageText = null;
        public LetterDef TriggerLetterDef = null;
        public Dictionary<string, ThingData> buildingLegend;
        public Dictionary<string, ThingData> nonbuildingLegend;
        public Dictionary<string, Rot4> rotationLegend;
        public Dictionary<string, ThingData> floorLegend;
        public Dictionary<string, ThingData> pawnLegend;
        public Dictionary<string, ThingData> itemLegend;
        public FactionDef factionDef = null;
    }
}
