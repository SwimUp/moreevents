using DiaRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class EmailMessageOption : IExposable
    {
        public abstract string Label { get; }

        public abstract void DoAction(EmailMessage message, EmailBox box, Pawn speaker);

        public virtual void ExposeData()
        {

        }
    }
}
