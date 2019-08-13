using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Gas
{
    public class Graphic_LinkedPipeOverlay : Graphic_Linked
    {
        public PipeType PipeType;

        public Graphic_LinkedPipeOverlay() : base()
        {

        }

        public Graphic_LinkedPipeOverlay(Graphic subGraphic) : base(subGraphic)
        {

        }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            return PipeAt(c, parent);
        }

        public override void Print(SectionLayer layer, Thing thing)
        {
            CellRect val = GenAdj.OccupiedRect(thing);
            foreach(var cell in val)
            {
                Vector3 vector = cell.ToVector3ShiftedWithAltitude(29);
                Printer_Plane.PrintPlane(layer, vector, Vector2.one, LinkedDrawMatFrom(thing, cell));
            }
        }

        public bool PipeAt(IntVec3 c, Thing parent)
        {
            if (!parent.Spawned)
                return false;

            if (!c.InBounds(parent.Map))
                return false;

            if (GridsUtility.GetTerrain(c, parent.Map).layerable)
                return false;

            if (!parent.Map.GetComponent<GasManager>().PipeAt(c, PipeType))
                return false;

            var comp = parent.TryGetComp<CompPipe>();
            if (comp != null || comp.PipeType == PipeType)
            {
                return true;
            }

            return false;
        }
    }
}
