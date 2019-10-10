using DarkNET.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Sly
{
    public class SlyHelp_OneUse : SlyService
    {
        protected bool alreadyUsed = false;

        public override string Label => "";

        public override string Description => "";

        public override IEnumerable<FloatMenuOption> Options(Map map)
        {
            yield break;
        }

        public override bool AvaliableRightNow(out string reason)
        {
            reason = "";

            if (alreadyUsed)
            {
                reason = "SlyService_AlreadyUsed".Translate();
                return false;
            }

            return true;
        }

        public override void SlyArrival(TraderWorker_Sly sly)
        {
            base.SlyArrival(sly);

            alreadyUsed = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref alreadyUsed, "alreadyUsed");
        }
    }
}
