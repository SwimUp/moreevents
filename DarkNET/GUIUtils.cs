using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public static class GUIUtils
    {
        public static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        public static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;

        public static bool DrawCustomButton(Rect rect, string label, Color textColor)
        {
            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(rect);
            GUI.color = CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(rect);
            return Widgets.ButtonInvisible(rect);
        }

        public static void DrawLineHorizontal(float x, float y, float length, Color color)
        {
            GUI.color = color;
            Widgets.DrawLineHorizontal(x, y, length);
            GUI.color = Color.white;
        }

        public static void DrawLineVertical(float x, float y, float length, Color color)
        {
            GUI.color = color;
            Widgets.DrawLineVertical(x, y, length);
            GUI.color = Color.white;
        }
    }
}
