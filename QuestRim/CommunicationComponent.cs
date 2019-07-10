using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace QuestRim
{
    public abstract class CommunicationComponent : IExposable, ILoadReferenceable
    {
        public int id;

        public virtual void Tick()
        {

        }

        public virtual void ExposeData()
        {

        }

        public string GetUniqueLoadID()
        {
            return "ComComponent_" + id;
        }
    }
}
