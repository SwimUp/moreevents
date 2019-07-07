using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public class QuestOption : IExposable
    {
        public string Label;

        public List<QuestAction> Actions;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Label, "Label");
            Scribe_Collections.Look(ref Actions, "Actions", LookMode.Deep);
        }
    }
}
