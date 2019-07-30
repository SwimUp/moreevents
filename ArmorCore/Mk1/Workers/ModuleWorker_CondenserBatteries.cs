using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ModuleWorker_CondenserBatteries : MKStationModule
    {
        public override void StationTick()
        {
            if (Find.TickManager.TicksGame % 400 == 0)
            {
                Station.TryChargeEnergyBank(def.EnergyBankCharge);
            }
        }

        public override string StatDescription()
        {
            return "ModuleWorker_CondenserBatteries_StatDescription".Translate(def.EnergyBankCharge);
        }
    }
}
