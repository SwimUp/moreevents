using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Constellations.Conditions
{
    public abstract class ConstellationCondition
    {
        public abstract bool CanSpawn(Map affectedMap);
    }
}
