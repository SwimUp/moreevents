﻿using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_EMP_Fox : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.ExplosiveBullets;

        public override float SeverityPerShot => 0.11f;

        public override bool FleshOnly => true;
    }
}
