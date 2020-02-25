using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things
{
    [StaticConstructorOnStartup]
    public class Gizmo_FillableMk1 : Gizmo
    {
        public Apparel_MkArmor Apparel;

        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_FillableMk1()
        {
            order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Text.Font = GameFont.Tiny;
                Rect rect2 = rect;
                if (Apparel.Active)
                {
                    rect2.height = overRect.height / 2f;
                    Widgets.Label(rect2, "EnergyChargeCapacity2".Translate());
                    Rect rect3 = rect;
                    rect3.yMin = overRect.height / 2f;
                    float fillPercent = Apparel.EnergyCharge;
                    Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect3,  $"{Apparel.EnergyCharge.ToString("f0")} / {Apparel.CoreComp.PowerCapacity}");
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                else
                {
                    Widgets.Label(rect2, "InactiveMk1".Translate());
                }

            });
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
