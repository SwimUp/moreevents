using RimArmorCore.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Mk1
{
    public class ArmorSlot : IExposable
    {
        public ArmorModuleCategory Category;

        public List<ModuleSlot<MKArmorModule>> Modules;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Category, "Category");
            Scribe_Collections.Look(ref Modules, "Modules", LookMode.Deep);
        }
    }

    public abstract class Apparel_MkArmor : Apparel
    {
        public virtual float EnergyCharge
        {
            set
            {
                CoreComp.EnergyCharge = value;
            }
            get
            {
                if (CoreComp == null)
                    return 0;
                else
                    return CoreComp.EnergyCharge;
            }
        }
        public virtual float dischargeRate { get; set; } = 0.35f;

        public bool FullCharge => EnergyCharge >= CoreComp.PowerCapacity;

        public abstract Apparel GetHelmet { get; }
        protected bool HasHelmet = false;

        public bool Active => Core != null && HasHelmet && EnergyCharge > 0f;

        public Thing Core;
        public ArmorCore CoreComp;

        public abstract int[] SlotsNumber { get; }

        public virtual List<ArmorSlot> Slots { get
            {
                if (slots == null)
                {
                    slots = new List<ArmorSlot>();
                    InitSlots();
                }

                return slots;
            } }

        protected List<ArmorSlot> slots;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            CoreComp = Core.TryGetComp<ArmorCore>();

            ModulesInit();

            Notify_ModulesChanges();
        }

        public void Notify_ModulesChanges()
        {
        }

        public virtual void ModulesInit()
        {
            foreach (var armorSlot in Slots)
            {
                foreach(var slot in armorSlot.Modules)
                {
                    if(slot.Module != null)
                    {
                        slot.Module.SetupStats(this);
                    }
                }
            }
        }

        public void AddModule(ArmorModuleDef moduleDef, Thing item)
        {
            MKArmorModule module = (MKArmorModule)Activator.CreateInstance(moduleDef.workerClass);
            module.def = moduleDef;
            module.Armor = this;

            foreach (var armorSlot in Slots)
            {
                if(armorSlot.Category == moduleDef.ModuleCategory)
                {
                    foreach(var slot in armorSlot.Modules)
                    {
                        if(moduleDef.ExcludesModules != null && slot.Module != null && moduleDef.ExcludesModules.Contains(slot.Module.def))
                        {
                            RemoveModule(slot.Module.def, true);
                        }

                        if (slot.Module == null)
                        {
                            slot.Module = module;
                            slot.Item = item;
                            Notify_ModulesChanges();
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveModule(ArmorModuleDef moduleDef)
        {
            foreach (var armorSlot in Slots)
            {
                if (armorSlot.Category == moduleDef.ModuleCategory)
                {
                    foreach (var slot in armorSlot.Modules)
                    {
                        if (slot.Module.def == moduleDef)
                        {
                            slot.Module = null;
                            break;
                        }
                    }
                }
            }

            Notify_ModulesChanges();
        }

        public void RemoveModule(ArmorModuleDef moduleDef, bool drop)
        {
            foreach (var armorSlot in Slots)
            {
                if (armorSlot.Category == moduleDef.ModuleCategory)
                {
                    foreach (var slot in armorSlot.Modules)
                    {
                        if (slot.Module.def == moduleDef)
                        {
                            slot.Module = null;

                            if(drop && slot.Item != null)
                            {
                                if (CellFinder.TryFindRandomCellNear(Position, Map, 4, null, out IntVec3 result))
                                {
                                    GenSpawn.Spawn(slot.Item, result, Map);
                                    slot.Item = null;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            Notify_ModulesChanges();
        }

        private void InitSlots()
        {
            int slot = 0;
            foreach(ArmorModuleCategory category in Enum.GetValues(typeof(ArmorModuleCategory)))
            {
                ArmorSlot armorSlot = new ArmorSlot();

                int slotsNumber = SlotsNumber[slot];

                var list = new List<ModuleSlot<MKArmorModule>>(slotsNumber);
                for (int i = 0; i < slotsNumber; i++)
                {
                    list.Add(new ModuleSlot<MKArmorModule>());
                }

                armorSlot.Category = category;
                armorSlot.Modules = list;

                Slots.Add(armorSlot);

                slot++;
            }
        }

        public static bool HasMk1Enable(Pawn p, ThingDef armorDef)
        {
            if (p.apparel == null)
                return false;

            foreach(var apparel in p.apparel.WornApparel)
            {
                if(apparel.def == armorDef)
                {
                    Apparel_MkArmor mk = (Apparel_MkArmor)apparel;
                    if (mk.Active)
                        return true;
                }
            }

            return false;
        }

        public static Apparel_MkArmor HasAnyMK(Pawn p)
        {
            if (p.apparel == null)
                return null;

            foreach (var apparel in p.apparel.WornApparel)
            {
                if (apparel is Apparel_MkArmor)
                {
                    return (Apparel_MkArmor)apparel;
                }
            }

            return null;
        }

        public void ChangeCore(Thing newCore)
        {
            Core = newCore;
            CoreComp = Core.TryGetComp<ArmorCore>();
        }

        public void AddCharge(float num)
        {
            if (Core == null)
                return;

            if (FullCharge)
                return;

            EnergyCharge += num;

            if (EnergyCharge > CoreComp.PowerCapacity)
                EnergyCharge = CoreComp.PowerCapacity;
        }

        public override string DescriptionDetailed
        {
            get
            {
                string text = base.GetInspectString();
                if (text.Length > 0)
                {
                    text += "\n";
                }
                if (Core == null)
                {
                    text += "NoCore".Translate();
                }
                else
                {
                    text += "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"), CoreComp.PowerCapacity);
                    if (!HasHelmet)
                        text += "InactiveNoHelmet".Translate();
                }

                return text;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref HasHelmet, "HasHelmet");
            Scribe_Deep.Look(ref Core, "Core");
            Scribe_Collections.Look(ref slots, "Slots", LookMode.Deep);
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (Active)
            {
                EnergyCharge -= dinfo.Amount * 0.1f;

                if(EnergyCharge < 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            float damage = dinfo.Amount * 0.8f;
            if (dinfo.Def == DamageDefOf.Cut || dinfo.Def == DamageDefOf.Blunt)
                damage *= 0.1f;

            dinfo.SetAmount(damage);

            absorbed = false;
        }

        public override void DrawWornExtras()
        {
            if (Find.TickManager.TicksGame % 200 == 0)
            {
                CheckHelmet();

                if (Active)
                {
                    EnergyCharge -= dischargeRate;

                    if (EnergyCharge < 0)
                        EnergyCharge = 0;
                }
            }
        }

        private void CheckHelmet()
        {
            if (Wearer == null)
            {
                HasHelmet = false;
                return;
            }

            if (GetHelmet != null)
            {
                HasHelmet = true;
            }
            else
            {
                HasHelmet = false;
            }
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            if (text.Length > 0)
            {
                text += "\n";
            }
            if (Core == null)
            {
                text += "NoCore".Translate();
            }
            else
            {
                text += "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"), CoreComp.PowerCapacity);
                if (!HasHelmet)
                    text += "InactiveNoHelmet".Translate();
            }

            return text;
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            yield return new Gizmo_FillableMk1
            {
              //  Apparel = this
            };
        }
    }
}
