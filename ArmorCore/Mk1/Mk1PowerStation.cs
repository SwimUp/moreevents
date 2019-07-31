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
        public float EnergyBankCharge;

        public float PowerLimit;

        public float OverDriveMultiplier => OverDriveEnabled ? 2f : 1f;
        public float OverDrive;
        public bool OverDriveEnabled;

        public bool CanOverDrive;

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

            Notify_ModulesChanges();
        }

        public override string GetInspectString()
        {
            return base.GetInspectString();
        }

        public void SetOverDrive(bool value)
        {
            if(!CanOverDrive)
            {
                Messages.Message("OverDrive_Disabled".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(!OverDriveEnabled && OverDrive > 0f)
            {
                Messages.Message("OverDrive_Wait".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            OverDriveEnabled = value;

            Messages.Message("OverDrive_ON".Translate(), MessageTypeDefOf.NeutralEvent);
        }

        public void Notify_ModulesChanges()
        {
            ChargeSpeed = 0.06f;
            EnergyBank = 0f;
            PowerLimit = 0f;

            for (int i = 0; i < Slots.Count; i++)
            {
                ModuleSlot slot = Slots[i];

                if (slot.Module != null)
                {
                    EnergyBank += slot.Module.def.EnergyBankCapacity;
                    ChargeSpeed += slot.Module.def.AdditionalChargeSpeed;
                    PowerLimit += slot.Module.def.PowerLimit;

                    if (!CanOverDrive)
                        CanOverDrive = slot.Module.def.EnableOverDrive;
                }
            }
        }

        public void AddModule(MKStationModuleDef moduleDef, Thing item)
        {
            MKStationModule module = (MKStationModule)Activator.CreateInstance(moduleDef.workerClass);
            module.def = moduleDef;
            module.Station = this;

            foreach (var slot in Slots)
            {
                if (slot.Module == null)
                {
                    slot.Module = module;
                    slot.Item = item;
                    Notify_ModulesChanges();
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

            Notify_ModulesChanges();
        }

        public bool TryChargeEnergyBank(float charge)
        {
            if (EnergyBank >= EnergyBankCharge)
                return false;

            EnergyBankCharge = Mathf.Clamp(EnergyBankCharge + charge, 0, EnergyBank);

            return true;
        }

        public override void Tick()
        {
            if(PowerLimit > 100f)
                return;

            if (Find.TickManager.TicksGame % 200 == 0)
            {
                if (HasArmor)
                {
                    var armor = (Apparel_Mk1)ContainedArmor;
                    float chargeCount = HasPower ? ChargeSpeed * OverDriveMultiplier : 0f;

                    if (!HasPower && EnergyBank > 0f)
                    {
                        chargeCount = Mathf.Min(EnergyBank, ChargeSpeed * OverDriveMultiplier);
                        EnergyBank -= chargeCount;
                    }

                    if (chargeCount == 0f)
                        return;

                    armor.AddCharge(ChargeSpeed);

                    OverDriveTick();
                }
            }

            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].Module?.StationTick();
            }
        }

        private void OverDriveTick()
        {
            if(!OverDriveEnabled)
            {
                OverDrive = Mathf.Clamp(OverDrive - 0.02f, 0, 100);
                return;
            }

            if(OverDrive >= 100f)
            {
                SetOverDrive(false);

                int totalHp = HitPoints;
                int totalDamage = Rand.Range(100, 400);
                if (totalHp - totalDamage <= 0)
                    totalDamage = 0;

                TakeDamage(new DamageInfo(DamageDefOf.Crush, totalDamage));
                return;
            }

            TakeDamage(new DamageInfo(DamageDefOf.Crush, Rand.Range(1, 2)));
            OverDrive += 0.08f;
        }

        public void OpenStation(Pawn pawn)
        {
            Find.WindowStack.Add(new MKStationWindow(this, pawn));
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
            Scribe_Values.Look(ref EnergyBankCharge, "EnergyBankCharge");
            Scribe_Values.Look(ref OverDriveEnabled, "OverDriveEnabled");
            Scribe_Values.Look(ref OverDrive, "OverDrive");
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
