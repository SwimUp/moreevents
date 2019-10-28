using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Hediffs
{
    public class HediffComp_Explode : HediffComp
    {
        private HediffCompProperties_Explode Props => (HediffCompProperties_Explode)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if(Pawn.IsHashIntervalTick(200))
            {
                if (parent.Severity >= Props.ExplodeSeverity)
                {
                    DoExplode();
                }
            }
        }

        public void DoExplode()
        {
            GenExplosion.DoExplosion(Pawn.Position, Pawn.Map, 1f, DamageDefOf.Bomb, null);

            if(Props.RemoveHediff)
            {
                Pawn.health.RemoveHediff(parent);
            }
        }
    }
}
