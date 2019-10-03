using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_SMP_42 : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            if (destination.InBounds(Map))
            {
                GenExplosion.DoExplosion(Position, Map, 1, DamageDefOf.Bomb, launcher);
            }

            base.Impact(hitThing);
        }
    }
}
