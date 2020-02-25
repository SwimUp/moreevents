using HarmonyLib;
using MoreEvents.Events;
using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(ExitMapGrid)), HarmonyPatch("MapUsesExitGrid", MethodType.Getter)]
    public class ExitCells_Patch
    {
        static bool Prefix(ExitMapGrid __instance, ref bool __result)
        {
            Map map = (Map)(typeof(ExitMapGrid).GetField("map", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Static).GetValue(__instance));

            if(map != null)
            {
                QuestSite site = Find.WorldObjects.WorldObjectAt<QuestSite>(map.Tile);
                if (site != null)
                {
                    __result = site.HasExitCells;
                    return false;
                }

                var comp = map.Parent.GetComponent<HasExitCellsComp>();
                if (comp != null)
                {
                    __result = false;
                    return false;
                }
            }

            return true;
        }
    }
}
