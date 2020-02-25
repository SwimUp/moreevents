using HarmonyLib;
using MoreEvents;
using RimOverhaul.Things.Stuff;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(Designator_Build))]
    [HarmonyPatch("DesignateSingleCell")]
    public class DesignateSingleCell
    {
        public static bool Prefix(Designator_Build __instance, ref IntVec3 c)
        {
            ThingDef stuffDef = (ThingDef)(typeof(Designator_Build).GetField("stuffDef", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                                                                    | BindingFlags.Static).GetValue(__instance));
            if (stuffDef != null)
            {
                if (stuffDef.comps != null)
                {
                    CompProperties_ReplaceThing compProperties_RepleceThing = stuffDef.comps.Where(x => x is CompProperties_ReplaceThing comp && comp.CompareThing == __instance.PlacingDef).FirstOrDefault() as CompProperties_ReplaceThing;
                    if (compProperties_RepleceThing != null)
                    {
                        Rot4 placingRot = (Rot4)(typeof(Designator_Build).GetField("placingRot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                                                        | BindingFlags.Static).GetValue(__instance));

                        if (DebugSettings.godMode || __instance.PlacingDef.GetStatValueAbstract(StatDefOf.WorkToBuild, stuffDef) == 0f)
                        {
                            if (__instance.PlacingDef is TerrainDef)
                            {
                                __instance.Map.terrainGrid.SetTerrain(c, (TerrainDef)__instance.PlacingDef);
                            }
                            else
                            {
                                Thing thing = ThingMaker.MakeThing(compProperties_RepleceThing.ReplaceThing, stuffDef);
                                thing.SetFactionDirect(Faction.OfPlayer);
                                GenSpawn.Spawn(thing, c, __instance.Map, placingRot);
                            }
                        }
                        else
                        {
                            BuildableDef buildableDef = compProperties_RepleceThing.ReplaceThing;
                            if (buildableDef.blueprintDef == null)
                            {
                                buildableDef.blueprintDef = ThingDefGenerator_BuildingsCustom.NewBlueprintDef_Thing(compProperties_RepleceThing.ReplaceThing, isInstallBlueprint: false);
                            }
                            if(buildableDef.frameDef == null)
                            {
                                buildableDef.frameDef = ThingDefGenerator_BuildingsCustom.NewFrameDef_Thing(compProperties_RepleceThing.ReplaceThing);
                            }

                            GenSpawn.WipeExistingThings(c, placingRot, buildableDef, __instance.Map, DestroyMode.Deconstruct);
                            GenConstruct.PlaceBlueprintForBuild(buildableDef, c, __instance.Map, placingRot, Faction.OfPlayer, stuffDef);
                        }

                        return false;
                    }
                }
            }

            return true;
        }
    }
}
