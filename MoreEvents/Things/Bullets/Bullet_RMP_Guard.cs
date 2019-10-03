using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_RMP_Guard : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.CryotiumShards;

        public override float SeverityPerShot => 0.07f;
    }
}
