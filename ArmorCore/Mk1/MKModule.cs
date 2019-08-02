using MoreEvents.Things.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore.Mk1
{
    public abstract class MKModule: IExposable
    {
        public virtual int SortOrder => 1;

        public virtual void Tick()
        {
        }

        public abstract string StatDescription();

        public abstract void ExposeData();
    }
}
