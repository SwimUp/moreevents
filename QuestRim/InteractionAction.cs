using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class InteractionAction : IExposable
    {
        public abstract void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant);

        public virtual void ExposeData()
        {

        }
    }
}
