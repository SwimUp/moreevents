using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommOption_PawnOfferYes : CommOption
    {
        public override string Label => "CommOption_PawnOfferYes_Label".Translate(OfferPawn.Name.ToStringFull);

        public Pawn OfferPawn;

        public CommOption_PawnOfferYes()
        {

        }

        public CommOption_PawnOfferYes(Pawn pawn)
        {
            OfferPawn = pawn;
        }

        public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            OfferPawn.SetFaction(Faction.OfPlayer);

            Find.LetterStack.ReceiveLetter("CommOption_PawnOfferYesTitle".Translate(), "CommOption_PawnOfferYes".Translate(OfferPawn.Name.ToStringFull, defendant.Faction.Name), LetterDefOf.PositiveEvent);

            defendant.Faction.TryAffectGoodwillWith(speaker.Faction, 20);

            if (defendant.GetQuestPawn(out QuestPawn questPawn))
            {
                if (questPawn.Dialogs.Contains(dialog))
                {
                    questPawn.Dialogs.Remove(dialog);
                    QuestsManager.Communications.RemoveCommunication(dialog);
                }
            }
            else
            {
                QuestsManager.Communications.RemoveCommunication(dialog);
            }
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref OfferPawn, "OfferPawn");
        }
    }
}
