using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_ThermoFusion : Bullet
    {
        private int ticksToExplosion = 0;
        
        public Bullet_ThermoFusion() : base()
        {
            ticksToExplosion = 15;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticksToExplosion, "ticksToExplosion");
        }

        public override void Tick()
        {
            base.Tick();

            if(ticksToExplosion <= 0)
            {
                ticksToExplosion = 10;

                DoSmallExplosion();
            }
            else
            {
                ticksToExplosion--;
            }
        }

        private void DoSmallExplosion()
        {
            GenExplosion.DoExplosion(Position, Map, Rand.Range(2,4), DamageDefOf.Flame, launcher, explosionSound: SoundDefOfLocal.Explosion_Flame);
        }

        protected override void Impact(Thing hitThing)
        {
            GenExplosion.DoExplosion(Position, Map, Rand.Range(6, 10), DamageDefOf.Bomb, launcher, Rand.Range(100, 500), 80);

            base.Impact(hitThing);
        }
    }
}
