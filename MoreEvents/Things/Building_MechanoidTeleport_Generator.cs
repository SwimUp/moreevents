using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreEvents.Things
{
    public class Building_MechanoidTeleport_Generator : Building
    {
        public bool Init = false;

        private Thing Teleport;

        private readonly List<DamageDef> dissallowDamage = new List<DamageDef>()
        {
            DamageDefOf.Cut,
            DamageDefOf.Arrow,
            DamageDefOf.Stab,
            DamageDefOf.Scratch,
            DamageDefOf.Bite,
            DamageDefOf.Rotting
        };

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (!Init)
            {
                if (!FindHomeAreaCell(map, out IntVec3 result) || Map.areaManager.Home.TrueCount <= 0)
                {
                    Destroy(0);
                }

                Teleport = GenSpawn.Spawn(ThingDefOfLocal.MechanoidTeleport, result, map);

                Init = true;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Teleport.Destroy(0);
            base.Destroy(mode);
        }

        private bool FindHomeAreaCell(Map map, out IntVec3 result)
        {
            List<IntVec3> cells = (from c in map.areaManager.Home.ActiveCells where c.GetRoof(map) != RoofDefOf.RoofRockThick && !c.Fogged(map) select c).ToList();
            if (cells.Count > 0)
            {
                result = cells.RandomElement();
                return true;
            }

            result = IntVec3.Invalid;
            return false;
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (dissallowDamage.Contains(dinfo.Def))
            {
                absorbed = true;
                return;
            }

            if(dinfo.Amount < 15)
            {
                absorbed = true;
                return;
            }

            base.PreApplyDamage(ref dinfo, out absorbed);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Init, "Init", false);
        }
    }
}
