using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class QuestAction : IExposable
    {
        public abstract void DoAction(Quest quest, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {

        }
    }
}
