using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Sly
{
    public abstract class SlyService : IExposable
    {
        public abstract string Label { get; }

        public abstract string Description { get; }

        public virtual void ExposeData()
        {
        }

        public virtual void SlyGone(TraderWorker_Sly sly)
        {

        }

        public virtual void SlyArrival(TraderWorker_Sly sly)
        {

        }

        public virtual void SlyDayPassed(TraderWorker_Sly sly)
        {

        }

        public virtual bool AvaliableRightNow(out string reason)
        {
            reason = "";

            return true;
        }

        public abstract IEnumerable<FloatMenuOption> Options(Map map);
    }
}
