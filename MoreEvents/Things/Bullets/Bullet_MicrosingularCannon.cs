using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_MicrosingularCannon : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            if(Map != null)
            {
                GenExplosion.DoExplosion(Position, Map, 4f, DamageDefOf.Bomb, launcher);

                GenSpawn.Spawn(ThingDefOfLocal.Singularity, Position, Map);
            }

            base.Impact(hitThing);
        }
    }
}
