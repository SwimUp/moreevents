using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Patch
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("RenderPawnAt", new[] { typeof(Vector3), typeof(RotDrawMode), typeof(bool) })]
    [StaticConstructorOnStartup]
    public class PawnUIOverlayPatch
    {
        private static readonly Material ExclamationPointMat = MaterialPool.MatFrom("UI/Overlays/QuestPoint", ShaderDatabase.MetaOverlay);

        static void Postfix(PawnRenderer __instance)
        {
            var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.GetQuestPawn(out QuestPawn questPawn))
            {
                if(questPawn.Quests.Count > 0)
                    RenderExclamationPointOverlay(pawn);
            }
        }

        private static void RenderExclamationPointOverlay(Thing t)
        {
            if (!t.Spawned) return;
            var drawPos = t.DrawPos;
            drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28125f;
            if (t is Pawn)
            {
                drawPos.x += (float)t.def.size.x - 1f;
                drawPos.z += (float)t.def.size.z + 0.2f;
            }
            RenderPulsingOverlayQuest(t, ExclamationPointMat, drawPos, MeshPool.plane05);
        }

        private static void RenderPulsingOverlayQuest(Thing thing, Material mat, Vector3 drawPos, Mesh mesh)
        {
            var num = (Time.realtimeSinceStartup + 397f * (float)(thing.thingIDNumber % 571)) * 4f;
            var num2 = ((float)Math.Sin((double)num) + 1f) * 0.5f;
            num2 = 0.3f + num2 * 0.7f;
            var material = FadedMaterialPool.FadedVersionOf(mat, num2);
            Graphics.DrawMesh(mesh, drawPos, Quaternion.identity, material, 0);
        }
    }
}
