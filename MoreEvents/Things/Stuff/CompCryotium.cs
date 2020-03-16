using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompCryotium : ThingComp
    {
        private DamageDef damageDef => DamageDefOf.Burn;

        private bool ShouldPushHeatNow
        {
            get
            {
                if (!parent.SpawnedOrAnyParentSpawned)
                {
                    return false;
                }
                float ambientTemperature = parent.AmbientTemperature;
                return ambientTemperature < 50 && ambientTemperature > -50;
            }
        }

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

            if (parent.IsHashIntervalTick(60) && ShouldPushHeatNow)
            {
                GenTemperature.PushHeat(parent.PositionHeld, parent.MapHeld, -8);
            }
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);

            if (signal.tag == "meleeattack")
            {
                try
                {
                    if (signal.args.TryGetArg(0, out NamedArgument arg1) && signal.args.TryGetArg(1, out NamedArgument arg2))
                    {
                        if (arg1.arg is Pawn target && arg2.arg is Pawn caster)
                        {
                            target.TakeDamage(new DamageInfo(DamageDefOf.Frostbite, Rand.Range(1, 5)));
                        }
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
