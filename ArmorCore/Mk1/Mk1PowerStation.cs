using RimArmorCore.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents.Things.Mk1
{
    [StaticConstructorOnStartup]
    public class Mk1PowerStation : Building
    {
        public int MaxModules => 3;

        public Thing ContainedArmor;

        public float ChargeSpeed = 0.06f;

        public List<ModuleSlot> Slots
        {
            get
            {
                if(slots == null)
                {
                    slots = new List<ModuleSlot>(MaxModules);
                    for(int i = 0; i < MaxModules; i++)
                    {
                        slots.Add(new ModuleSlot());
                    }
                    AddModule(MKStationModuleDefOfLocal.CondenserBatteries);
                }

                return slots;
            }
        }
        private List<ModuleSlot> slots;

        public bool HasPower
        {
            get
            {
                if ((power != null && power.PowerOn))
                {
                    return !this.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                }
                return false;
            }
        }

        public bool HasArmor => ContainedArmor != null;

        private static Graphic DisableTex = null;
        private static Graphic EnableTex = null;

        public CompPowerTrader power;

        public float EnergyBank;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            power = GetComp<CompPowerTrader>();

            LongEventHandler.ExecuteWhenFinished((Action)CreateAnim);

            if(ContainedArmor != null)
            {
                Apparel_Mk1 mk1 = (Apparel_Mk1)ContainedArmor;
                mk1.CoreComp = mk1.Core.TryGetComp<ArmorCore>();
            }

            ChargeSpeed = 0.06f;
            EnergyBank = 0f;
            for (int i = 0; i < Slots.Count; i++)
            {
                ModuleSlot slot = Slots[i];

                if (slot.Module != null)
                {
                    EnergyBank += slot.Module.def.EnergyBankCapacity;
                    ChargeSpeed += slot.Module.def.AdditionalChargeSpeed;
                }
            }
        }

        public void AddModule(MKStationModuleDef moduleDef)
        {
            MKStationModule module = (MKStationModule)Activator.CreateInstance(moduleDef.workerClass);
            module.def = moduleDef;
            module.Station = this;

            foreach (var slot in Slots)
            {
                if (slot.Module == null)
                {
                    slot.Module = module;
                    break;
                }
            }
        }

        public void RemoveModule(MKStationModuleDef moduleDef)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                ModuleSlot slot = Slots[i];
                if(slot.Module.def == moduleDef)
                {
                    slot.Module = null;
                }
            }
        }

        public bool TryChargeEnergyBank(float charge)
        {
            if (EnergyBank >= 100f)
                return false;

            EnergyBank = Mathf.Clamp(EnergyBank + charge, 0, 100);

            return true;
        }

        public override void Tick()
        {
            if (Find.TickManager.TicksGame % 200 == 0)
            {
                if (HasArmor)
                {
                    var armor = (Apparel_Mk1)ContainedArmor;

                    if (HasPower)
                    {
                        armor.AddCharge(ChargeSpeed);
                    }
                    else if(EnergyBank > 0f)
                    {
                        float chargeCount = Mathf.Min(EnergyBank, ChargeSpeed);
                        EnergyBank -= chargeCount;

                        armor.AddCharge(chargeCount);
                    }
                }
            }

            for(int i = 0; i < Slots.Count; i++)
            {
                Slots[i].Module?.StationTick();
            }
        }

        public void OpenStation()
        {
            Find.WindowStack.Add(new MKStationWindow(this));
        }

        private void CreateAnim()
        {
            DisableTex = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath, this.def.graphicData.Graphic.Shader);
            DisableTex.drawSize = this.def.graphicData.drawSize;

            EnableTex = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath + "_enable", this.def.graphicData.Graphic.Shader);
            EnableTex.drawSize = this.def.graphicData.drawSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref ContainedArmor, "ContainedArmor");
            Scribe_Collections.Look(ref slots, "Slots", LookMode.Deep);
            Scribe_Values.Look(ref EnergyBank, "EnergyBank");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("OpenMKStation".Translate(), delegate
            {
                Job job = new Job(RimArmorCore.JobDefOfLocal.OpenStation, this);
                selPawn.jobs.TryTakeOrderedJob(job);
            });
        }

        public override void Draw()
        {
            if (HasArmor && HasPower && EnableTex != null)
            {
                Matrix4x4 matrix = default;
                Vector3 s = new Vector3(4f, 1f, 4f);
                matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Rotation.AsQuat, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, EnableTex.MatAt(Rotation, null), 0);
            }
        }
    }
}
