using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class CompProperties_ArmorCore : CompProperties
    {
        public float PowerCapacity;

        public ThingDef Fuel;

        public float FuelConsumption;

        public float ChargingSpeed;

        public int MaxFuel;

        public float HeatSpeed;
        public bool CanOverHeat;
        public float CoolingRate;

        public CompProperties_ArmorCore()
        {
            compClass = typeof(ArmorCore);
        }
    }
}
