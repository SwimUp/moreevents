using RimArmorCore.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
        public virtual float dischargeRate { get; set; } = 0.20f;

        public bool FullCharge => EnergyCharge >= CoreComp.PowerCapacity;

        public abstract Apparel GetHelmet { get; }
        protected bool HasHelmet = false;

        public bool Active => Core != null && HasHelmet && EnergyCharge > 0f;

        public bool CanHeal;
        public float HealRate;
        protected virtual float healRate { get; }

        public Thing Core;
        public ArmorCore CoreComp
        {
            get
            {
                if (coreComp == null)
                {
                    coreComp = Core.TryGetComp<ArmorCore>();
                }

                return coreComp;
            }
            set
            {
                coreComp = value;
            }
        }
        private ArmorCore coreComp = null;

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

        private List<MKArmorModule> damageListeners = new List<MKArmorModule>();

        public IEnumerable<MKArmorModule> StatsListeners => statsListeners;
        private List<MKArmorModule> statsListeners = new List<MKArmorModule>();
        private List<MKArmorModule> wornExtraListeners = new List<MKArmorModule>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            ModulesInit();

            Notify_ModulesChanges();
        }

        public override void Tick()
        {
            base.Tick();

            for (int i = 0; i < wornExtraListeners.Count; i++)
            {
                MKArmorModule listener = wornExtraListeners[i];

                listener.Tick();
            }
        }

        public void Notify_ModulesChanges()
        {
            damageListeners.Clear();
            statsListeners.Clear();

            HealRate = 0;

            foreach (var armorSlot in Slots)
            {
                foreach (var slot in armorSlot.Modules)
                {
                    if (slot.Module != null)
                    {
                        HealRate += slot.Module.def.HealRate;

                        if(slot.Module.def.DamageListener)
                        {
                            damageListeners.Add(slot.Module);
                        }

                        if(slot.Module.def.StatListener)
                        {
                            statsListeners.Add(slot.Module);
                        }

                        if(slot.Module.def.WornExtraListener)
                        {
                            wornExtraListeners.Add(slot.Module);
                        }
                    }
                }
            }

            if (HealRate > 0f)
                CanHeal = true;
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

        public void RemoveModule(ArmorModuleDef moduleDef, IntVec3 pos, Map map, bool drop)
        {
            foreach (var armorSlot in Slots)
            {
                if (armorSlot.Category == moduleDef.ModuleCategory)
                {
                    foreach (var slot in armorSlot.Modules)
                    {
                        if (slot.Module != null)
                        {
                            if (slot.Module.def == moduleDef)
                            {
                                slot.Module = null;

                                if (drop && slot.Item != null)
                                {
                                    if (CellFinder.TryFindRandomCellNear(pos, map, 4, null, out IntVec3 result))
                                    {
                                        GenSpawn.Spawn(slot.Item, result, map);
                                        slot.Item = null;
                                    }
                                }
                                break;
                            }
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
            coreComp = null;
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

            Notify_ModulesChanges();

            ModulesInit();
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            bool absorb = false;

            for(int i = 0; i < damageListeners.Count; i++)
            {
                MKArmorModule listener = damageListeners[i];

                listener.CheckPreAbsorbDamage(dinfo, ref absorb);
            }

            return absorb;
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (Active)
            {
                EnergyCharge = Mathf.Clamp(EnergyCharge - (dinfo.Amount * 0.3f), 0, CoreComp.PowerCapacity);
            }

            absorbed = false;
        }

        public override void DrawWornExtras()
        {
            for (int i = 0; i < wornExtraListeners.Count; i++)
            {
                MKArmorModule listener = wornExtraListeners[i];

                listener.DrawWornExtras();
            }

            if (Find.TickManager.TicksGame % 200 == 0)
            {
                CheckHelmet();

                if (Active)
                {
                    EnergyCharge = Mathf.Clamp(EnergyCharge - dischargeRate, 0, EnergyCharge);

                    if (CanHeal)
                    {
                        TryHeal();
                    }
                }
            }
        }

        private void TryHeal()
        {
            if (Wearer == null)
                return;

            if ((from x in Wearer.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.CanHealNaturally() || x.CanHealFromTending()
                 select x).TryRandomElement(out Hediff_Injury result2))
            {
                result2.Heal(HealRate);
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
                Apparel = this
            };

            foreach (var armorSlot in Slots)
            {
                foreach (var slot in armorSlot.Modules)
                {
                    if (slot.Module != null)
                    {
                        foreach(var gizmo in slot.Module.GetGizmos())
                        {
                            yield return gizmo;
                        }
                    }
                }
            }
        }
    }
}
