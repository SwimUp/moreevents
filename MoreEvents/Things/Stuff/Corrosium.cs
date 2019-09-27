using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class Corrosium : Building
    {
        private float progress = 0f;

        private float progressSpeed => 0.009f;

        public float MaxProgress => 0.5f;

        public override void TickLong()
        {
            if (progress == MaxProgress)
                return;

            progress = Mathf.Clamp(progress + progressSpeed, 0, MaxProgress);
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);

            float damage = (1 - progress) * dinfo.Amount;

            dinfo.SetAmount(damage);
        }

        public override string GetInspectString()
        {
            float damageReduces = (1 - progress) * 100;

            return $"Corrosium_InspectString".Translate(damageReduces.ToString("f2"));
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref progress, "progress");
        }
    }
}
