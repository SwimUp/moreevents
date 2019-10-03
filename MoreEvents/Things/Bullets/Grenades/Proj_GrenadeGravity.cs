using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets.Grenades
{
    public class Proj_GrenadeGravity : Projectile_Explosive
    {
        protected override void Explode()
        {
            IntVec3 center = Position;
            Map map = Map;

            IEnumerable<IntVec3> cells = GenRadial.RadialCellsAround(center, def.projectile.explosionRadius + 2f, false);
            foreach(var cell in cells)
            {
                List<Thing> things = cell.GetThingList(map);
                for(int i = 0; i < things.Count; i++)
                {
                    Pawn p = things[i] as Pawn;
                    if(p != null)
                    {
                        p.Position = center;
                    }
                }
            }

            base.Explode();
        }
    }
}
