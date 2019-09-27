using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompMetalFB15 : ThingComp
    {
        private float regenRate = 0;

        public float RegenRate
        {
            get
            {
                if (regenRate == 0)
                {
                    regenRate = parent.MaxHitPoints * 0.007f;
                }

                return regenRate;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            if(!parent.def.useHitPoints)
            {
                parent.AllComps.Remove(this);

                return;
            }

            if(parent.def.tickerType == TickerType.Never)
                parent.def.tickerType = TickerType.Normal;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(parent.IsHashIntervalTick(500))
            {
                TryRegenHealth();
            }
        }

        private void TryRegenHealth()
        {
            if (parent.HitPoints == parent.MaxHitPoints)
                return;

            parent.HitPoints = (int)Mathf.Clamp(parent.HitPoints + RegenRate, 1, parent.MaxHitPoints);
        }
    }
}
