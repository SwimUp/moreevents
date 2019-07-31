using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class ModuleSlot : IExposable
    {
        public int Order => Module == null ? 1 : Module.SortOrder;

        public MKStationModule Module;

        public Thing Item;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Module, "Module");
            Scribe_Deep.Look(ref Item, "Item");
        }
    }
}
