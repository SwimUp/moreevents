using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_WR_Wisp : Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            if(Map != null)
            {
                CellRect cellRect = CellRect.CenteredOn(base.Position, 7);
                cellRect.ClipInsideMap(Map);

                for (int i = 0; i < Rand.Range(4, 7); i++)
                {
                    IntVec3 randomCell = cellRect.RandomCell;
                    Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Map, randomCell));
                }

                GenExplosion.DoExplosion(Position, Map, 5, DamageDefOf.Bomb, launcher, Rand.Range(35, 50), 50);

                IncidentParms parms = new IncidentParms
                {
                    target = Map
                };

                IncidentDefOfLocal.IceStorm.Worker.TryExecute(parms);
            }

            base.Impact(hitThing);
        }
    }
}
