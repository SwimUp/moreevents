using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public class MKStationModule : IExposable
    {
        public MKStationModuleDef def;

        public Mk1PowerStation Station;

        public virtual int SortOrder => 1;

        public virtual void StationTick()
        {
        }

        public virtual string StatDescription()
        {
            return "";
        }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_References.Look(ref Station, "Station");
        }
    }
}
