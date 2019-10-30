using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_Hediff : Bullet
    {
        public virtual HediffDef HediffDef { get; }

        public virtual float SeverityPerShot { get; }

        public virtual bool FleshOnly => false;

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if(hitThing != null)
            {
                Pawn pawn = hitThing as Pawn;
                if (pawn != null)
                {
                    if (FleshOnly && pawn.RaceProps.IsMechanoid)
                        return;

                    HealthUtility.AdjustSeverity(pawn, HediffDef, SeverityPerShot);
                }
            }
        }
    }
}
