using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_PG_18 : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.ExposureChargedPlasma;

        public override float SeverityPerShot => 0.05f;
    }
}
