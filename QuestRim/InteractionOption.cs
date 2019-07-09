using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class InteractionOption : IExposable
    {
        public string Label;

        public List<InteractionAction> Actions;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Label, "Label");
            Scribe_Collections.Look(ref Actions, "Actions", LookMode.Deep);
        }
    }
}
