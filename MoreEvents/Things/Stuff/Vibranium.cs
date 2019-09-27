using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class Vibranium : ThingWithComps
    {
        public CompVibranium CompVibranium
        {
            get
            {
                if (compVibranium == null)
                {
                    compVibranium = GetComp<CompVibranium>();
                }

                return compVibranium;
            }
        }

        private CompVibranium compVibranium;


        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);

            if (CompVibranium.Props.AbsorbDamage.TryGetValue(dinfo.Def, out float value))
            {
                dinfo.SetAmount(dinfo.Amount * value);
            }
        }
    }
}
