using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreEvents.Events
{
    public class DoesntHasExitCells_Comp : WorldObjectCompProperties
    {
        public DoesntHasExitCells_Comp()
        {
            compClass = typeof(HasExitCellsComp);
        }
    }
}
