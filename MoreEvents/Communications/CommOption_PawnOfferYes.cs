using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_PawnOfferYes : CommOption
    {
        public override string Label => "CommOption_PawnOfferYes_Label".Translate(OfferPawn.Name.ToStringShort);

        public Pawn OfferPawn;

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            throw new NotImplementedException();
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref OfferPawn, "OfferPawn");
        }
    }
}
