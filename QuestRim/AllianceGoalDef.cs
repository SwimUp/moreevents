using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class AllianceGoalDef : Def
    {
        public bool PlayerOnly;

        public Type workerClass;

        public List<StorytellerCompProperties> comps = new List<StorytellerCompProperties>();
    }
}
