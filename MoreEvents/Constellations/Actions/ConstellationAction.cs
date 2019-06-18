using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreEvents.Constellations.Actions
{
    public abstract class ConstellationAction
    {
        public abstract void GiveEffect();

        public abstract void RemoveEffects();
    }
}
