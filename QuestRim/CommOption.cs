using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class CommOption : IExposable
    {
        public abstract string Label { get; }

        public abstract void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {

        }
    }
}
