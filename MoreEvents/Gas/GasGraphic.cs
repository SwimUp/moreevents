using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    [StaticConstructorOnStartup]
    public static class GasGraphic
    {
        public static string[] GraphicPath = new string[]
        {
            "Things/Buildings/GasPipe/PipeOverlay_Atlas",
            "Things/Buildings/GasPipe/PipeOverlay_Atlas"
        };
        public static ColorInt[] GraphicColor = new ColorInt[]
        {
            new ColorInt(255, 255, 255),
            new ColorInt(255, 255, 255)
        };

        public static string[] GraphicPathForGrid = new string[]
        {
            "Things/Buildings/GasPipe/PipeOverlay_Atlas",
            "Things/Buildings/GasPipe/PipeOverlay_Atlas"
        };
        public static ColorInt[] GraphicColorForGrid = new ColorInt[]
        {
            new ColorInt(0, 255,0),
            new ColorInt(255, 0 , 0)
        };

        public static Graphic_LinkedPipe[] PipeGraphic;
        public static Graphic_LinkedPipeOverlay[] PipeOverlayGraphic;

        static GasGraphic()
        {
            int length = Enum.GetValues(typeof(PipeType)).Length;

            PipeGraphic = new Graphic_LinkedPipe[length];
            PipeOverlayGraphic = new Graphic_LinkedPipeOverlay[length];

            for (int i = 0; i < PipeGraphic.Length; i++)
            {
                PipeGraphic[i] = new Graphic_LinkedPipe(GraphicDatabase.Get<Graphic_Single>(GraphicPath[i], ShaderDatabase.Transparent, Vector2.one, GraphicColor[i].ToColor))
                {
                    PipeType = (PipeType)i
                };

                PipeOverlayGraphic[i] = new Graphic_LinkedPipeOverlay(GraphicDatabase.Get<Graphic_Single>(GraphicPathForGrid[i], ShaderDatabase.MetaOverlay, Vector2.one, GraphicColorForGrid[i].ToColor))
                {
                    PipeType = (PipeType)i
                };
            }
        }
    }
}
