using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_RL_SteelRain : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            if(Map != null)
            {
                GenExplosion.DoExplosion(Position, Map, 4, DamageDefOf.Bomb, launcher);
            }

            base.Impact(hitThing);
        }
    }
}
