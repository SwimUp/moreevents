using RimArmorCore.Mk1;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore
{
    public static class Utils
    {
        public static string GetLabel(this ArmorModuleCategory category)
        {
            switch(category)
            {
                case ArmorModuleCategory.Body:
                    return "ArmorModuleCategory_Body".Translate();
                case ArmorModuleCategory.General:
                    return "ArmorModuleCategory_General".Translate();
                case ArmorModuleCategory.Head:
                    return "ArmorModuleCategory_Head".Translate();
                case ArmorModuleCategory.Legs:
                    return "ArmorModuleCategory_Legs".Translate();
            }

            return "";
        }

        public static string[] ToArrayValues<T,D>(this Dictionary<T, D> dictionary)
        {
            string[] result = new string[dictionary.Count];

            for(int i = 0; i < dictionary.Count; i++)
            {
                D value = dictionary.ElementAt(i).Value;
                result[i] = value.ToString();
            }

            return result;
        }

        public static void DoExplosionPlus(IntVec3 center, Predicate<IntVec3> validator, Map map, float radius, DamageDef damType, Thing instigator, int damAmount = -1, float armorPenetration = -1f, SoundDef explosionSound = null, ThingDef weapon = null, ThingDef projectile = null, Thing intendedTarget = null, ThingDef postExplosionSpawnThingDef = null, float postExplosionSpawnChance = 0f, int postExplosionSpawnThingCount = 1, bool applyDamageToExplosionCellsNeighbors = false, ThingDef preExplosionSpawnThingDef = null, float preExplosionSpawnChance = 0f, int preExplosionSpawnThingCount = 1, float chanceToStartFire = 0f, bool damageFalloff = false)
        {
            if (map == null)
            {
                Log.Warning("Tried to do explosion in a null map.");
                return;
            }
            if (damAmount < 0)
            {
                damAmount = damType.defaultDamage;
                armorPenetration = damType.defaultArmorPenetration;
                if (damAmount < 0)
                {
                    Log.ErrorOnce("Attempted to trigger an explosion without defined damage", 91094882);
                    damAmount = 1;
                }
            }
            if (armorPenetration < 0f)
            {
                armorPenetration = (float)damAmount * 0.015f;
            }
            ExplosionPlus explosion = (ExplosionPlus)GenSpawn.Spawn(ThingDefOfLocal.ExplosionPlus, center, map);
            explosion.radius = radius;
            explosion.damType = damType;
            explosion.instigator = instigator;
            explosion.damAmount = damAmount;
            explosion.armorPenetration = armorPenetration;
            explosion.weapon = weapon;
            explosion.projectile = projectile;
            explosion.intendedTarget = intendedTarget;
            explosion.preExplosionSpawnThingDef = preExplosionSpawnThingDef;
            explosion.preExplosionSpawnChance = preExplosionSpawnChance;
            explosion.preExplosionSpawnThingCount = preExplosionSpawnThingCount;
            explosion.postExplosionSpawnThingDef = postExplosionSpawnThingDef;
            explosion.postExplosionSpawnChance = postExplosionSpawnChance;
            explosion.postExplosionSpawnThingCount = postExplosionSpawnThingCount;
            explosion.applyDamageToExplosionCellsNeighbors = applyDamageToExplosionCellsNeighbors;
            explosion.chanceToStartFire = chanceToStartFire;
            explosion.damageFalloff = damageFalloff;
            explosion.validator = validator;
            explosion.StartExplosion(explosionSound, new List<Thing>());
        }

    }
}
