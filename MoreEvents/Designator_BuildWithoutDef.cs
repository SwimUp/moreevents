using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MoreEvents
{
    public class Designator_BuildWithoutDef : Designator_Build
    {
        private ThingDef stuffDef;

        public Designator_BuildWithoutDef(ThingDef def) : base(def)
        {
            ResetStuff();

            if(entDef.blueprintDef == null)
            {
                entDef.blueprintDef = ThingDefGenerator_BuildingsCustom.NewBlueprintDef_Thing(def, isInstallBlueprint: false);
            }
            if(entDef.frameDef == null)
            {
                entDef.frameDef = ThingDefGenerator_BuildingsCustom.NewFrameDef_Thing(def);
            }
        }

        private void ResetStuff()
        {
            ThingDef thingDef = entDef as ThingDef;
            if (thingDef != null && thingDef.MadeFromStuff)
            {
                stuffDef = GenStuff.DefaultStuffFor(thingDef);
            }
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            if (DebugSettings.godMode || entDef.GetStatValueAbstract(StatDefOf.WorkToBuild, stuffDef) == 0f)
            {
                if (entDef is TerrainDef)
                {
                    base.Map.terrainGrid.SetTerrain(c, (TerrainDef)entDef);
                }
                else
                {
                    Thing thing = ThingMaker.MakeThing((ThingDef)entDef, stuffDef);
                    thing.SetFactionDirect(Faction.OfPlayer);
                    GenSpawn.Spawn(thing, c, base.Map, placingRot);
                }
            }
            else
            {
                GenSpawn.WipeExistingThings(c, placingRot, entDef.blueprintDef, base.Map, DestroyMode.Deconstruct);
                GenConstruct.PlaceBlueprintForBuild(entDef, c, base.Map, placingRot, Faction.OfPlayer, stuffDef);
            }
            MoteMaker.ThrowMetaPuffs(GenAdj.OccupiedRect(c, placingRot, entDef.Size), base.Map);
            ThingDef thingDef = entDef as ThingDef;
            if (thingDef != null && thingDef.IsOrbitalTradeBeacon)
            {
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BuildOrbitalTradeBeacon, KnowledgeAmount.Total);
            }
            if (TutorSystem.TutorialMode)
            {
                TutorSystem.Notify_Event(new EventPack(base.TutorTagDesignate, c));
            }
            if (entDef.PlaceWorkers != null)
            {
                for (int i = 0; i < entDef.PlaceWorkers.Count; i++)
                {
                    entDef.PlaceWorkers[i].PostPlace(base.Map, entDef, c, placingRot);
                }
            }
        }
    }
}
