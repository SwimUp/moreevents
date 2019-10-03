using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_PG_26 : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.EnergyOverload;

        public override float SeverityPerShot => 0.05f;

        protected override void Impact(Thing hitThing)
        {
            GenExplosion.DoExplosion(Position, Map, 1, DamageDefOf.Bomb, launcher, 5);

            base.Impact(hitThing);
        }
    }
}
