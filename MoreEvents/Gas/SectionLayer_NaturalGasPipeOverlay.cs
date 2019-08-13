using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class SectionLayer_NaturalGasPipeOverlay : SectionLayer_Things
    {
        public PipeType PipeType;

        public SectionLayer_NaturalGasPipeOverlay(Section section) : base(section)
        {
            relevantChangeTypes = MapMeshFlag.Buildings;
            requireAddToMapMesh = false;
            PipeType = PipeType.NaturalGas;
        }

        public override void DrawLayer()
        {
            if (Find.DesignatorManager.SelectedDesignator is Designator_Build des)
            {
                if (des.PlacingDef is ThingDef thingDef && thingDef.comps != null && thingDef.comps.Any(x => x is CompProperties_GasPipe x2 && x2.pipeType == PipeType))
                {
                    base.DrawLayer();
                }
            }
        }

        protected override void TakePrintFrom(Thing t)
        {
            var comp = t.TryGetComp<CompPipe>();
            if (comp != null && comp.PipeType == PipeType)
            {
                comp.PrintForGrid(this);
            }
        }
    }
}
