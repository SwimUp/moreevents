using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things
{
    public class Building_WorkTable_AnimatedHeatPush : Building_WorkTable_Animated
    {
        public override void UsedThisTick()
        {
            base.UsedThisTick();
            if (Find.TickManager.TicksGame % 30 == 4)
            {
                GenTemperature.PushHeat(this, def.building.heatPerTickWhileWorking * 30f);
            }
        }
    }
}
