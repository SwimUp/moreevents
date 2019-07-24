using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.AI
{
    public class JobGiver_AIDefendLeaderPawn : JobGiver_AIDefendPawn
    {
        protected override Pawn GetDefendee(Pawn pawn)
        {
            Pawn target = pawn.mindState.duty.focus.Thing as Pawn;

            return target;
        }
    }
}
