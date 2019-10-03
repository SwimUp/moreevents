using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_DeathRay : Bullet
    {
        public override void Tick()
        {
            base.Tick();

            if(Map != null)
            {
                FireUtility.TryStartFireIn(Position, Map, Rand.Range(0.4f, 0.8f));
            }
        }
    }
}
