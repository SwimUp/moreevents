﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1.Workers
{
    public class ArmorModuleWorker_SimpleSolarPanels : ArmorModuleWorker_SolarPanel
    {
        protected override int chargeRate => 50;

        protected override float chargeNum => 0.03f;
    }
}
