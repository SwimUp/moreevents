using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompShockCrystals : ThingComp
    {
        private DamageDef damageDef => DamageDefOf.EMP;

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);

            if (parent is Building)
            {
                if (dinfo.Def != damageDef)
                    GenExplosion.DoExplosion(parent.Position, parent.Map, Rand.Range(3, 5), damageDef, parent);
            }
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);

            if (signal.tag == "meleeattack")
            {
                try
                {
                    Pawn target = signal.args[0] as Pawn;
                    Pawn caster = signal.args[1] as Pawn;

                    if (target != null && caster != null)
                    {
                        GenExplosion.DoExplosion(target.Position, target.Map, 1.5f, DamageDefOf.EMP, caster);
                    }
                }
                catch (Exception ex)
                {
                    Log.Message("MeleeAttack error [CompShockCrystals]" + ex);
                }
            }
        }
    }
}
