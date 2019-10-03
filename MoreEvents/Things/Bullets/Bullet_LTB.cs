using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_LTB : Bullet
    {
        private int ticksToExplosion = 0;
        
        public Bullet_LTB() : base()
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
                ticksToExplosion = 4;

                DoEffect(Map, Position);
            }
            else
            {
                ticksToExplosion--;
            }
        }

        private void DoEffect(Map map, IntVec3 pos)
        {
            if (map != null)
            {
                if (GenRadial.RadialCellsAround(pos, 7, false).TryRandomElement(out IntVec3 newPos))
                {
                    if (newPos.InBounds(map))
                        map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(map, newPos));
                }
            }
        }

        protected override void Impact(Thing hitThing)
        {
            if (Map != null)
            {
                int raidus = Rand.Range(5, 7);

                GenExplosion.DoExplosion(Position, Map, raidus, DamageDefOf.EMP, launcher, explosionSound: SoundDefOfLocal.Explosion_Flame);
                GenExplosion.DoExplosion(Position, Map, raidus, DamageDefOf.Bomb, launcher, damAmount: Rand.Range(20, 50), armorPenetration: 50, explosionSound: SoundDefOfLocal.Explosion_Flame);
            }

            base.Impact(hitThing);
        }
    }
}
