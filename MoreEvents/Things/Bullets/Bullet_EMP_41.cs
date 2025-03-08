﻿using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Bullets
{
    public class Bullet_EMP_41 : Bullet_Hediff
    {
        public override HediffDef HediffDef => HediffDefOfLocal.InternalBleeding;

        public override float SeverityPerShot => 0.08f;

        public override bool FleshOnly => true;
    }
}
