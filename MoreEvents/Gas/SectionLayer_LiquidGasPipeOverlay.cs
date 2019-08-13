using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class SectionLayer_LiquidGasPipeOverlay : SectionLayer_GasPipeOverlay
    {
        public override PipeType PipeType => PipeType.LiquidGas;

        public SectionLayer_LiquidGasPipeOverlay(Section section) : base(section)
        {
            relevantChangeTypes = MapMeshFlag.Buildings;
            requireAddToMapMesh = false;
        }
    }
}
