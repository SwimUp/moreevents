using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things
{
    public class Building_GasHeater : Building
    {
        public CompTempControl compTempControl;

        public CompRefuelable compRefuelable;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compTempControl = GetComp<CompTempControl>();
            compRefuelable = GetComp<CompRefuelable>();
        }

        public override void Tick()
        {
            if (this.IsHashIntervalTick(250))
            {
                if (compRefuelable.HasFuel)
                {
                    float ambientTemperature = base.AmbientTemperature;
                    float num = (ambientTemperature < 20f) ? 1f : ((!(ambientTemperature > 120f)) ? Mathf.InverseLerp(120f, 20f, ambientTemperature) : 0f);
                    float energyLimit = compTempControl.Props.energyPerSecond * num * 4.16666651f;
                    float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, compTempControl.targetTemperature);
                    bool flag = !Mathf.Approximately(num2, 0f);
                    compTempControl.operatingAtHighPower = flag;
                    this.GetRoomGroup().Temperature += num2;
                }
            }
        }
    }
}
