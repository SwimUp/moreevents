using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Gas
{
    public class CompGasStation : CompPipe
    {
        public CompProperties_GasStation Props => (CompProperties_GasStation)props;

        public Building_GasStation ParentStation;

        public override void NetInit()
        {
            base.NetInit();

            ParentStation = (Building_GasStation)parent;

            ParentStation.Reload();
        }
    }
}
