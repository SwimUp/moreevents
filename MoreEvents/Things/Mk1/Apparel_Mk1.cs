using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class Apparel_Mk1 : Apparel
    {
        public float EnergyCharge = 100f;
        public float dischargeRate = 0.23f;

        public Apparel GetHelmet => Wearer.apparel.WornApparel.Where(a => a.def == ThingDefOfLocal.Apparel_MK1ThunderHead).FirstOrDefault();
        private bool HasHelmet = false;

        public bool Active => HasHelmet && EnergyCharge > 0f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref HasHelmet, "HasHelmet");
            Scribe_Values.Look(ref EnergyCharge, "EnergyCharge");
        }

        public override void TickRare()
        {
            base.TickRare();

            CheckHelmet();

            if (Active)
            {
                EnergyCharge -= dischargeRate;

                if (EnergyCharge < 0)
                    EnergyCharge = 0;
            }
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
        }

        private void CheckHelmet()
        {
            if (Wearer == null)
            {
                HasHelmet = false;
                return;
            }

            if (GetHelmet != null)
                HasHelmet = true;
            else
                HasHelmet = false;
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            if (Wearer != null)
            {
                if (text.Length > 0)
                {
                    text += "\n";
                }
                text += "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"));
            }

            return text;
        }
    }
}
