using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Pawns
{
    public class Pawn_Goliath : Pawn
    {
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);

            if (dinfo.Def == DamageDefOf.Bomb || dinfo.Def == DamageDefOf.Flame)
                return;

            if (Rand.Chance(0.7f))
                absorbed = true;
        }
    }
}
