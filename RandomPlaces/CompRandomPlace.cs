using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RandomPlaces
{
    public abstract class CompRandomPlace : IExposable
    {
        public virtual void ExposeData()
        {

        }

        public abstract void PostMapGenerate(Map map);
    }
}
