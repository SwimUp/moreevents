using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class SectionLayer_NaturalGasPipeOverlay : SectionLayer_GasPipeOverlay
    {
        public override PipeType PipeType => PipeType.NaturalGas;

        public SectionLayer_NaturalGasPipeOverlay(Section section) : base(section)
        {
            relevantChangeTypes = MapMeshFlag.Buildings;
            requireAddToMapMesh = false;
        }
    }
}
