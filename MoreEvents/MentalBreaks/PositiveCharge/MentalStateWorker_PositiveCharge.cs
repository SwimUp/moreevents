using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MoreEvents.MentalBreaks.PositiveCharge
{
    public class MentalStateWorker_PositiveCharge : MentalStateWorker
    {
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!base.StateCanOccur(pawn))
            {
                return false;
            }

            return true;
        }
    }
}
