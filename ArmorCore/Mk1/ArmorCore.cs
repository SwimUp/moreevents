using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class ArmorCore : ThingComp
    {
        public float EnergyCharge = 0f;

        public CompProperties_ArmorCore Props => (CompProperties_ArmorCore)props;

        public float PowerCapacity => Props.PowerCapacity;

        public float Fuel;

        public float OverHeat;

        public bool UseReactor = true;

        private bool isOverHeat = false;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref EnergyCharge, "EnergyCharge");
            Scribe_Values.Look(ref Fuel, "Fuel");
            Scribe_Values.Look(ref OverHeat, "OverHeat");
            Scribe_Values.Look(ref UseReactor, "UseReactor");
            Scribe_Values.Look(ref isOverHeat, "isOverHeat");
        }

        public override void CompTick()
        {
            base.CompTick();

            if (Find.TickManager.TicksGame % 200 == 0)
            {
                if(isOverHeat || !UseReactor)
                {
                    OverHeat = Mathf.Clamp(OverHeat - Props.CoolingRate, 0, 100);

                    if(OverHeat <= 50)
                    {
                        isOverHeat = false;
                    }

                    return;
                }

                if (Fuel > 0)
                {
                    Fuel = Mathf.Clamp(Fuel - Props.FuelConsumption, 0, Fuel);
                    EnergyCharge += Props.ChargingSpeed;

                    if(Props.CanOverHeat)
                    {
                        OverHeat = Mathf.Clamp(OverHeat + Props.HeatSpeed, 0, 100);
                        if(OverHeat == 100)
                        {
                            isOverHeat = true;
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action()
            {
                defaultLabel = "Core_ArmorCore_OverHeat".Translate(UseReactor ? "UseReactorON".Translate() : "UseReactorNO".Translate(), OverHeat),
                defaultDesc = "Core_ArmorCore_OverHeat_Desc".Translate(Props.Fuel.LabelCap, Fuel, Props.MaxFuel),
                icon = parent.def.uiIcon,
                action = delegate
                {
                    UseReactor = !UseReactor;
                }
            };
        }

        public void AddFuel(int count)
        {
            Fuel = Mathf.Clamp(Fuel + count, 0, Props.MaxFuel);
        }

        public override string CompInspectStringExtra()
        {
            string result = "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"), PowerCapacity);
            if(Props.Fuel != null)
            {
                result += "Core_Fuel".Translate(UseReactor ? "UseReactorON".Translate() : "UseReactorNO".Translate(), Fuel, Props.MaxFuel, Props.Fuel.LabelCap); 
            }

            return result;
        }
    }
}

