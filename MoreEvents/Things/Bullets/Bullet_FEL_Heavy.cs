using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_FEL_Heavy : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.ElectromagneticShock;

        public override float SeverityPerShot => 0.1f;
    }
}
