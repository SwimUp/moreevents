using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Things.Stuff
{
    public class CompAddedComp : ThingComp
    {
        public CompProperties_AddedThing Props => (CompProperties_AddedThing)props;
    }
}
