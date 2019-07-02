using Harmony;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Raid
{
    [HarmonyPatch(typeof(StatWorker_MarketValue))]
    [HarmonyPatch("GetValueUnfinalized")]
    public class Raid
    {
        static bool Prefix(StatWorker_MarketValue __instance, ref StatRequest req, ref float __result)
        {
            if (req.Thing is Pawn)
            {
                __result = 1f;
                return false;
            }

            return true;
        }
    }

    [StaticConstructorOnStartup]
    public class RaidHook : Mod
    {
        internal static HarmonyInstance harmonyInstance;

        public RaidHook(ModContentPack content) : base(content)
        {
            harmonyInstance = HarmonyInstance.Create("net.funkyshit.RaidHook");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
