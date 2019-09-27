using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompCorrosium : ThingComp
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            if (!parent.def.useHitPoints || parent is Building)
            {
                parent.AllComps.Remove(this);

                return;
            }

            if (parent.def.tickerType == TickerType.Never)
                parent.def.tickerType = TickerType.Normal;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(parent.IsHashIntervalTick(60000))
            {
                DoDamage();
            }
        }

        private void DoDamage()
        {
            parent.TakeDamage(new DamageInfo(DamageDefOf.Crush, parent.HitPoints * 0.05f));
        }
    }
}
