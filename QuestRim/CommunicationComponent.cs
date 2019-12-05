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

        public virtual void Notify_WarIsStarted(War war)
        {

        }

        public virtual void Notify_WarIsOver(War war)
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
        }

        public string GetUniqueLoadID()
        {
            return "ComComponent_" + id;
        }
    }
}
