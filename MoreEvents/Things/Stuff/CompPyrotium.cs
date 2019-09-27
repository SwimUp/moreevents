using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompPyrotium : ThingComp
    {
        private DamageDef damageDef => DamageDefOf.Burn;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            if (parent.def.tickerType == TickerType.Never)
                parent.def.tickerType = TickerType.Normal;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.IsHashIntervalTick(1000))
                parent.TakeDamage(new DamageInfo(DamageDefOf.Crush, parent.MaxHitPoints * 0.01f));
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);

            if (parent is Building && dinfo.Instigator != null && dinfo.Instigator != parent)
            {
                if (dinfo.Def != damageDef)
                    GenExplosion.DoExplosion(parent.Position, parent.Map, Rand.Range(2, 4), damageDef, parent, Rand.Range(9, 18));
            }
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);

            if (signal.tag == "meleeattack")
            {
                try
                {
                    if (signal.args[0] is Pawn target && signal.args[1] is Pawn caster)
                    {
                        target.TakeDamage(new DamageInfo(DamageDefOf.Flame, Rand.Range(1, 5)));
                    }
                }
                catch (Exception ex)
                {
                    Log.Message("MeleeAttack error [CompPyrotium]" + ex);
                }
            }
        }
    }
}
