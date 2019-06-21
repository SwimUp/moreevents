using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class ArmorCore : ThingComp
    {
        public float EnergyCharge = 0f;

        public CompProperties_ArmorCore Props => (CompProperties_ArmorCore)props;

        public string StationLabel => Props.StationLabel.Translate();

        public float PowerCapacity => Props.PowerCapacity;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref EnergyCharge, "EnergyCharge");
        }

        public override string CompInspectStringExtra()
        {
            return "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"), PowerCapacity);
        }
    }
}

