using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents.Things.Mk1;
using RimWorld;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_SolarPanel : MKArmorModule
    {
        protected virtual int chargeRate => 0;

        protected virtual float chargeNum => 0f;

        private bool active = false;

        public override string StatDescription()
        {
            return "ArmorModuleWorker_SolarPanel".Translate(chargeNum);
        }

        public override void SetupStats(Apparel_MkArmor armor)
        {
            base.SetupStats(armor);

            CheckCore();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            CheckCore();
        }

        private void CheckCore()
        {
            if (Armor != null)
            {
                if (Armor.CoreComp != null)
                {
                    if (Armor.CoreComp.Props.Fuel == null)
                    {
                        active = true;
                    }
                }
            }
        }

        public override void Notify_CoreChanged(Thing newCore)
        {
            base.Notify_CoreChanged(newCore);

            CheckCore();
        }

        public override void Tick()
        {
            base.Tick();

            if (active)
            {
                if (Find.TickManager.TicksGame % chargeRate == 0)
                {
                    if (!Armor.FullCharge && Armor.Wearer != null)
                    {
                        float num = GenCelestial.CurCelestialSunGlow(Armor.Wearer.Map);
                        if (GenCelestial.IsDaytime(num))
                        {
                            Armor.AddCharge(chargeNum);
                        }
                    }
                }
            }
        }
    }
}
