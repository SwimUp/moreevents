using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Things
{
    [StaticConstructorOnStartup]
    public class SkyfallerPlus : Skyfaller
    {
        public Action ImpactAction;

        protected override void Impact()
        {
            base.Impact();

            ImpactAction?.Invoke();
        }
    }
}
