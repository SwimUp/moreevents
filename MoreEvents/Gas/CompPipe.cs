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

        public PipeType PipeType => GasProps.pipeType;

        public CompProperties_GasPipe GasProps => (CompProperties_GasPipe)props;

        public PipelineNet pipeNet;

        public GasManager GasManager;

        private int pipeTypeInt = 0;

        public CompPowerTrader compPowerTrader;

        public static bool PlayerHasGeoscape = false;

        public bool HasPower
        {
            get
            {
                if (compPowerTrader != null && compPowerTrader.PowerOn)
                {
                    return !parent.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                }
                return false;
            }
        }

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

        public virtual void NetInit()
        {

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

            compPowerTrader = parent.GetComp<CompPowerTrader>();

            GasManager = parent.Map.GetComponent<GasManager>();

            GasManager.RegisterPipe(this, respawningAfterLoad);

            pipeTypeInt = (int)PipeType;

            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
