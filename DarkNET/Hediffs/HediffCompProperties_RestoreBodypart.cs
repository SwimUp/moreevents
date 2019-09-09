using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Hediffs
{
    public class HediffCompProperties_RestoreBodypart : HediffCompProperties
    {
        public List<BodyPartDef> HealableParts;

        public SimpleCurve ChancePerSeverity;

        public HediffCompProperties_RestoreBodypart()
        {
            compClass = typeof(HediffComp_RestoreBodypart);
        }
    }
}
