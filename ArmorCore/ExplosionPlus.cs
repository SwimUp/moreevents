using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimArmorCore
{
    public class ExplosionPlus : Explosion
    {
        private int startTick;

        private List<IntVec3> cellsToAffect;

        private List<Thing> damagedThings;

        private HashSet<IntVec3> addedCellsAffectedOnlyByDamage;

        private List<Thing> ignoredThings;

        private const float DamageFactorAtEdge = 0.2f;

        private static HashSet<IntVec3> tmpCells = new HashSet<IntVec3>();

        public Predicate<IntVec3> validator;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                cellsToAffect = SimplePool<List<IntVec3>>.Get();
                cellsToAffect.Clear();
                damagedThings = SimplePool<List<Thing>>.Get();
                damagedThings.Clear();
                addedCellsAffectedOnlyByDamage = SimplePool<HashSet<IntVec3>>.Get();
                addedCellsAffectedOnlyByDamage.Clear();
            }
        }

        public override void StartExplosion(SoundDef explosionSound, List<Thing> ignoredThings)
        {
            if (!base.Spawned)
            {
                Log.Error("Called StartExplosion() on unspawned thing.");
                return;
            }
            startTick = Find.TickManager.TicksGame;
            this.ignoredThings = ignoredThings;
            cellsToAffect.Clear();
            damagedThings.Clear();
            addedCellsAffectedOnlyByDamage.Clear();
            cellsToAffect.AddRange(damType.Worker.ExplosionCellsToHit(this));
            if (applyDamageToExplosionCellsNeighbors)
            {
                AddCellsNeighbors(cellsToAffect);
            }

            if (validator != null)
            {
                for (int i = 0; i < cellsToAffect.Count; i++)
                {
                    IntVec3 cell = cellsToAffect[i];

                    if (validator(cell))
                    {
                        cellsToAffect.Remove(cell);
                    }
                }
            }

            damType.Worker.ExplosionStart(this, cellsToAffect);
            PlayExplosionSound(explosionSound);
            MoteMaker.MakeWaterSplash(base.Position.ToVector3Shifted(), base.Map, radius * 6f, 20f);
            cellsToAffect.Sort((IntVec3 a, IntVec3 b) => GetCellAffectTick(b).CompareTo(GetCellAffectTick(a)));
            RegionTraverser.BreadthFirstTraverse(base.Position, base.Map, (Region from, Region to) => true, delegate (Region x)
            {
                List<Thing> allThings = x.ListerThings.AllThings;
                for (int num = allThings.Count - 1; num >= 0; num--)
                {
                    if (allThings[num].Spawned)
                    {
                        allThings[num].Notify_Explosion(this);
                    }
                }
                return false;
            }, 25);
        }
        public override void Tick()
        {
            int ticksGame = Find.TickManager.TicksGame;
            int count = cellsToAffect.Count;
            int num = count - 1;
            while (num >= 0 && ticksGame >= GetCellAffectTick(cellsToAffect[num]))
            {
                try
                {
                    AffectCell(cellsToAffect[num]);
                }
                catch (Exception ex)
                {
                    Log.Error("Explosion could not affect cell " + cellsToAffect[num] + ": " + ex);
                }
                cellsToAffect.RemoveAt(num);
                num--;
            }
            if (!cellsToAffect.Any())
            {
                Destroy();
            }
        }
        private bool ShouldCellBeAffectedOnlyByDamage(IntVec3 c)
        {
            if (!applyDamageToExplosionCellsNeighbors)
            {
                return false;
            }
            return addedCellsAffectedOnlyByDamage.Contains(c);
        }

        private void AffectCell(IntVec3 c)
        {
            if (c.InBounds(base.Map))
            {
                bool flag = ShouldCellBeAffectedOnlyByDamage(c);
                if (!flag && Rand.Chance(preExplosionSpawnChance) && c.Walkable(base.Map))
                {
                    TrySpawnExplosionThing(preExplosionSpawnThingDef, c, preExplosionSpawnThingCount);
                }
                damType.Worker.ExplosionAffectCell(this, c, damagedThings, ignoredThings, !flag);
                if (!flag && Rand.Chance(postExplosionSpawnChance) && c.Walkable(base.Map))
                {
                    TrySpawnExplosionThing(postExplosionSpawnThingDef, c, postExplosionSpawnThingCount);
                }
                float num = chanceToStartFire;
                if (damageFalloff)
                {
                    num *= Mathf.Lerp(1f, 0.2f, c.DistanceTo(base.Position) / radius);
                }
                if (Rand.Chance(num))
                {
                    FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
                }
            }
        }

        private int GetCellAffectTick(IntVec3 cell)
        {
            return startTick + (int)((cell - base.Position).LengthHorizontal * 1.5f);
        }


        private void TrySpawnExplosionThing(ThingDef thingDef, IntVec3 c, int count)
        {
            if (thingDef != null)
            {
                if (thingDef.IsFilth)
                {
                    FilthMaker.TryMakeFilth(c, base.Map, thingDef, count);
                    return;
                }
                Thing thing = ThingMaker.MakeThing(thingDef);
                thing.stackCount = count;
                GenSpawn.Spawn(thing, c, base.Map);
            }
        }

        private void PlayExplosionSound(SoundDef explosionSound)
        {
            if ((!Prefs.DevMode) ? (!explosionSound.NullOrUndefined()) : (explosionSound != null))
            {
                explosionSound.PlayOneShot(new TargetInfo(base.Position, base.Map));
            }
            else
            {
                damType.soundExplosion.PlayOneShot(new TargetInfo(base.Position, base.Map));
            }
        }

        private void AddCellsNeighbors(List<IntVec3> cells)
        {
            tmpCells.Clear();
            addedCellsAffectedOnlyByDamage.Clear();
            for (int i = 0; i < cells.Count; i++)
            {
                tmpCells.Add(cells[i]);
            }
            for (int j = 0; j < cells.Count; j++)
            {
                if (!cells[j].Walkable(base.Map))
                {
                    continue;
                }
                for (int k = 0; k < GenAdj.AdjacentCells.Length; k++)
                {
                    IntVec3 intVec = cells[j] + GenAdj.AdjacentCells[k];
                    if (intVec.InBounds(base.Map) && tmpCells.Add(intVec))
                    {
                        addedCellsAffectedOnlyByDamage.Add(intVec);
                    }
                }
            }
            cells.Clear();
            foreach (IntVec3 tmpCell in tmpCells)
            {
                cells.Add(tmpCell);
            }
            tmpCells.Clear();
        }
    }
}
