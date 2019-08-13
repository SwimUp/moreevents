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

        private CompProperties_GasPipe Props => (CompProperties_GasPipe)props;

        public PipelineNet pipeNet;

        public GasManager GasManager;

        private int pipeTypeInt = 0;

        public int GridID
        {
            get;
            set;
        } = -1;

        public override void PostDeSpawn(Map map)
        {
            GasManager.DeregisterPipe(this);
            pipeNet.DeregisterPipe(parent);
            base.PostDeSpawn(map);
        }


        public override string CompInspectStringExtra()
        {
            if (DebugSettings.godMode)
            {
                return PipeType.ToString() + "_ID:" + GridID;
            }
            return null;
        }

        public void PrintForGrid(SectionLayer layer)
        {
            GasGraphic.PipeOverlayGraphic[pipeTypeInt].Print(layer, parent);
        }

        public virtual void PipelineNet()
        {

        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            compFlickable = parent.GetComp<CompFlickable>();

            GasManager = parent.Map.GetComponent<GasManager>();

            GasManager.RegisterPipe(this, respawningAfterLoad);

            pipeTypeInt = (int)PipeType;

            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
