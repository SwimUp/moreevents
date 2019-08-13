using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class SectionLayer_GasPipeOverlay : SectionLayer_Things
    {
        public virtual PipeType PipeType { get; }

        public SectionLayer_GasPipeOverlay(Section section) : base(section)
        {
            relevantChangeTypes = MapMeshFlag.Buildings;
            requireAddToMapMesh = false;
        }

        public override void DrawLayer()
        {
            if (Find.DesignatorManager.SelectedDesignator is Designator_Build des)
            {
                if (des.PlacingDef is ThingDef thingDef && thingDef.comps != null && thingDef.comps.Any(x => x is CompProperties_GasPipe x2 && (x2.pipeType == PipeType || x2.ConnectEverything)))
                {
                     base.DrawLayer();
                }
            }
        }

        protected override void TakePrintFrom(Thing t)
        {
            var comp = t.TryGetComp<CompPipe>();
            if (comp != null && (comp.PipeType == PipeType || comp.GasProps.ConnectEverything))
            {
                comp.PrintForGrid(this);
            }
        }
    }
}
