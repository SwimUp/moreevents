using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_ArgentRocket : Projectile_Explosive
    {
        protected override void Impact(Thing hitThing)
        {
            Map map = Map;
            if (map != null)
            {
                IntVec3 position = base.Position;
                Map map2 = map;
                float explosionRadius = def.projectile.explosionRadius;
                DamageDef bomb = DamageDefOf.Bomb;
                Thing launcher = base.launcher;
                int damageAmount = base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                ThingDef equipmentDef = base.equipmentDef;
                GenExplosion.DoExplosion(position, map2, explosionRadius, bomb, launcher, damageAmount, armorPenetration, null, equipmentDef, def, intendedTarget.Thing);

                CellRect cellRect = CellRect.CenteredOn(base.Position, 15);
                cellRect.ClipInsideMap(map);

                for (int i = 0; i < 15; i++)
                {
                    IntVec3 randomCell = cellRect.RandomCell;
                    Projectile projectile = (Projectile)GenSpawn.Spawn(ThingDefOfLocal.Bullet_ArgentSphere, position, Map);

                    projectile.Launch(launcher, Position.ToVector3(), randomCell, randomCell, ProjectileHitFlags.All);
                }
            }

            base.Impact(hitThing);
        }
    }
}
