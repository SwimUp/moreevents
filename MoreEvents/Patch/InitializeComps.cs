using Harmony;
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
    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("InitializeComps")]
    public class InitializeComps
    {
        public static void Postfix(ThingWithComps __instance)
        {
            if(__instance.Stuff != null && __instance.Stuff.comps != null)
            {
                CompProperties_AddedThing compProperties_AddedThing = __instance.Stuff.comps.Where(x => x is CompProperties_AddedThing).FirstOrDefault() as CompProperties_AddedThing;
                if(compProperties_AddedThing != null)
                {
                    if (!compProperties_AddedThing.ForApparel && __instance is Apparel)
                        return;

                    if(!compProperties_AddedThing.ForWeapons && __instance.def.equipmentType == EquipmentType.Primary)
                        return;

                    FieldInfo compsField = (typeof(ThingWithComps).GetField("comps", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                                                                    | BindingFlags.Static));
                    List<ThingComp> comps = compsField.GetValue(__instance) as List<ThingComp>;
                    if(comps == null)
                    {
                        comps = new List<ThingComp>();
                    }

                    ThingComp thingComp = (ThingComp)Activator.CreateInstance(compProperties_AddedThing.Comp);
                    thingComp.parent = __instance;
                    comps.Add(thingComp);
                    thingComp.Initialize(compProperties_AddedThing);

                    compsField.SetValue(__instance, comps);
                }
            }
        }
    }
}
