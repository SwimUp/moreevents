using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasPowerPlant : CompPowerTrader
    {
        protected CompBreakdownable breakdownableComp;

        protected CompGasPowerPlantSettings compGasPowerPlantSettings;

        private PipelineNet pipelineNet => compGasPowerPlantSettings.pipeNet;

        private float gasConsumePerTick;

        protected virtual float DesiredPowerOutput => 0f - base.Props.basePowerConsumption;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            breakdownableComp = parent.GetComp<CompBreakdownable>();
            if (base.Props.basePowerConsumption < 0f && !parent.IsBrokenDown() && FlickUtility.WantsToBeOn(parent))
            {
                base.PowerOn = true;
            }
            compGasPowerPlantSettings = parent.GetComp<CompGasPowerPlantSettings>();

            gasConsumePerTick = compGasPowerPlantSettings.Props.GasConsumption / 60000;
        }

        public override void CompTick()
        {
            base.CompTick();
            UpdateDesiredPowerOutput();
        }

        public void UpdateDesiredPowerOutput()
        {
            if ((breakdownableComp != null && breakdownableComp.BrokenDown) || (flickableComp != null && !flickableComp.SwitchIsOn) || !base.PowerOn || !pipelineNet.GetGasFromNet(gasConsumePerTick))
            {
                base.PowerOutput = 0f;
            }
            else
            {
                base.PowerOutput = DesiredPowerOutput;
            }
        }
    }
}
