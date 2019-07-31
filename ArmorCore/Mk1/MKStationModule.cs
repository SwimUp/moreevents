using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class MKStationModule : IExposable
    {
        public MKStationModuleDef def;

        public Mk1PowerStation Station;

        public virtual int SortOrder => 1;

        public virtual void StationTick()
        {
        }

        public virtual string StatDescription()
        {
            StringBuilder builder = new StringBuilder();
            if(def.PowerLimit > 0f)
            {
                builder.AppendLine("ModuleWorker_StatDescription_PowerLimit".Translate(def.PowerLimit));
            }
            if(def.AdditionalChargeSpeed > 0f)
            {
                builder.AppendLine("ModuleWorker_StatDescription_AddCharge".Translate(def.AdditionalChargeSpeed));
            }
            if(def.EnergyBankCharge > 0f)
            {
                builder.AppendLine("ModuleWorker_StatDescription_Charging".Translate(def.EnergyBankCharge));
            }
            if(def.EnergyBankCapacity > 0f)
            {
                builder.AppendLine("ModuleWorker_StatDescription_Capacity".Translate(def.EnergyBankCapacity));
            }
            return builder.ToString();
        }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_References.Look(ref Station, "Station");
        }
    }
}
