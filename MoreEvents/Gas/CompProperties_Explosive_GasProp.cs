using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompProperties_Explosive_GasProp : CompProperties
    {
        public float explosiveRadius = 1.9f;

        public DamageDef explosiveDamageType;

        public int damageAmountBase = -1;

        public float armorPenetrationBase = -1f;

        public ThingDef postExplosionSpawnThingDef;

        public float postExplosionSpawnChance;

        public int postExplosionSpawnThingCount = 1;

        public bool applyDamageToExplosionCellsNeighbors;

        public ThingDef preExplosionSpawnThingDef;

        public float preExplosionSpawnChance;

        public int preExplosionSpawnThingCount = 1;

        public float chanceToStartFire;

        public bool damageFalloff;

        public float explosiveExpandPerStackcount;

        public float explosiveExpandPerFuel;

        public EffecterDef explosionEffect;

        public SoundDef explosionSound;

        public DamageDef startWickOnDamageTaken;

        public float startWickHitPointsPercent = 0.2f;

        public IntRange wickTicks = new IntRange(140, 150);

        public float wickScale = 1f;

        public float chanceNeverExplodeFromDamage;

        public float destroyThingOnExplosionSize;

        public DamageDef requiredDamageTypeToExplode;

        public CompProperties_Explosive_GasProp()
        {
            compClass = typeof(CompExplosive_GasProp);
        }
    }
}
