using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET
{
    public class DarkNetTrader : IExposable
    {
        public DarkNetTraderDef def;

        public List<Thing> Goods;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Collections.Look(ref Goods, "Goods", LookMode.Reference);
        }
    }
}
