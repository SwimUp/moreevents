using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class QuestDef : Def
    {
        public Type Quest;
        public float Commonality;
        public IncidentDef Incident;
    }
}
