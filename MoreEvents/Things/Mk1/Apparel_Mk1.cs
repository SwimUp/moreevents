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
        public float EnergyCharge = 0f;
        public float dischargeRate = 0.35f;

        public bool FullCharge => EnergyCharge >= 100f;

        public Apparel GetHelmet => Wearer.apparel.WornApparel.Where(a => a.def == ThingDefOfLocal.Apparel_MK1ThunderHead).FirstOrDefault();
        private bool HasHelmet = false;

        public bool Active => HasHelmet && EnergyCharge > 0f;

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

        public void AddCharge(float num)
        {
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
                text += "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"));
                if (!HasHelmet)
                    text += "InactiveNoHelmet".Translate();

                return text;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref HasHelmet, "HasHelmet");
            Scribe_Values.Look(ref EnergyCharge, "EnergyCharge");
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
            text += "EnergyChargeCapacity".Translate(EnergyCharge.ToString("f2"));
            if (!HasHelmet)
                text += "InactiveNoHelmet".Translate();

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
