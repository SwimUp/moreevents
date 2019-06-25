using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreEvents.MentalBreaks.PositiveCharge
{
    public class MentalState_PositiveCharge : MentalState
    {
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.SuperActive;
        }

        public override void PreStart()
        {
            base.PreStart();

            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.PositiveCharge);
        }


        public override void PostStart(string reason)
        {
            base.PostStart(reason);

            RecoverFromState();
        }

        public override string GetBeginLetterText()
        {
            return def.beginLetter.Formatted(pawn.LabelShort).AdjustedFor(pawn).CapitalizeFirst();
        }
    }
}
