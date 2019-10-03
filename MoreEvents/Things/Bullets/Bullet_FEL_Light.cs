using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_FEL_Light : Bullet_Hediff
    {
        public override float SeverityPerShot => 0.05f;

        public override HediffDef HediffDef => HediffDefOfLocal.ElectromagneticShock;
    }
}
