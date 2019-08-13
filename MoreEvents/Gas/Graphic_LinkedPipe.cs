using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class Graphic_LinkedPipe : Graphic_Linked
    {
        public PipeType PipeType;

        public Graphic_LinkedPipe() : base()
        {

        }

        public Graphic_LinkedPipe(Graphic subGraphic) : base(subGraphic)
        {

        }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            return PipeAt(c, parent);
        }

        public bool PipeAt(IntVec3 c, Thing parent)
        {
            if (!parent.Spawned)
                return false;

            if (!c.InBounds(parent.Map))
                return false;

            if(GridsUtility.GetTerrain(c, parent.Map).layerable)
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
