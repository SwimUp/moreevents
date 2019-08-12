using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompPipe : ThingComp
    {
        public CompFlickable compFlickable;

        public PipeType PipeType => Props.pipeType;

        public CompProperties_GasPipe Props => (CompProperties_GasPipe)props;

        public PipelineNet pipeNet;

        public int GridID
        {
            get;
            set;
        } = -1;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            compFlickable = parent.GetComp<CompFlickable>();

            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
