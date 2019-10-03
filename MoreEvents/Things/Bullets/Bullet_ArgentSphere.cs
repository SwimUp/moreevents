using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_ArgentSphere : Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            if(Map != null)
            {
                Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Map, Position));

                GenExplosion.DoExplosion(Position, Map, 5, DamageDefOf.Flame, launcher, Rand.Range(40, 70), 25, SoundDefOfLocal.Explosion_Flame);
            }

            base.Impact(hitThing);
        }
    }
}
