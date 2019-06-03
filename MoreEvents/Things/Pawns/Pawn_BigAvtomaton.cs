using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things.Pawns
{
    public class Pawn_BigAvtomaton : Pawn
    {
        private Map currentMap;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            currentMap = map;
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);

            if(Dead)
            {
                Explode();
            }
        }

        private void Explode()
        {
            GenExplosion.DoExplosion(this.Position, currentMap, 4, DamageDefOf.Bomb, null);
        }
    }
}
