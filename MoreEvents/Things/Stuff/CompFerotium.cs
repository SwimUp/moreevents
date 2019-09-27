using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents;
using RimWorld;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompFerotium : ThingComp
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            if (parent is Building && parent.def.tickerType == TickerType.Never)
            {
                parent.def.tickerType = TickerType.Normal;
            }
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            if(signal.tag == "meleeattack")
            {
                try
                {
                    Pawn target = signal.args[0] as Pawn;
                    Pawn caster = signal.args[1] as Pawn;

                    if (target != null && caster != null)
                    {
                        HealthUtility.AdjustSeverity(target, HediffDefOfLocal.PoisonHit, 0.07f);
                    }
                }
                catch(Exception ex)
                {
                    Log.Message("MeleeAttack error [CompFerotium]" + ex);
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.IsHashIntervalTick(30000))
            {
                IEnumerable<Thing> growable = GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, 2, false).Where(x => x is Plant);
                foreach(var thing in growable)
                {
                    Plant plant = thing as Plant;
                    if (plant.LifeStage == PlantLifeStage.Growing)
                        plant.Growth += plant.GrowthRate * 0.1f;
                }

                parent.TakeDamage(new DamageInfo(DamageDefOf.Crush, 40));
            }
        }
    }
}
