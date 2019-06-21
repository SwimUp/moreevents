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
        public float EnergyCharge
        {
            set
            {
                CoreComp.EnergyCharge = value;
            }
            get
            {
                return CoreComp.EnergyCharge;
            }
        }
        public float dischargeRate = 0.35f;

        public bool FullCharge => EnergyCharge >= CoreComp.PowerCapacity;

        public Apparel GetHelmet => Wearer.apparel.WornApparel.Where(a => a.def == ThingDefOfLocal.Apparel_MK1ThunderHead).FirstOrDefault();
        private bool HasHelmet = false;

        public bool Active => Core != null && HasHelmet && EnergyCharge > 0f;

        public Thing Core;
        public ArmorCore CoreComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            CoreComp = Core.TryGetComp<ArmorCore>();
        }

        public static bool HasMk1Enable(Pawn p)
        {
            if (p.apparel == null)
                return false;

            foreach(var apparel in p.apparel.WornApparel)
            {
                if(apparel.def == ThingDefOfLocal.Apparel_MK1Thunder)
                {
                    Apparel_Mk1 mk1 = (Apparel_Mk1)apparel;
                    if (mk1.Active)
                        return true;
                }
            }

            return false;
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

            if (EnergyCharge > 100f)
                EnergyCharge = 100f;
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
                Apparel = this
            };
        }
    }
}
