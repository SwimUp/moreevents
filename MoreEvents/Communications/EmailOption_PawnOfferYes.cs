using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class EmailOption_PawnOfferYes : EmailMessageOption
    {
        public override string Label => "CommOption_PawnOfferYes_Label".Translate(OfferPawn.Name.ToStringFull);

        public Pawn OfferPawn;

        public EmailOption_PawnOfferYes()
        {

        }

        public EmailOption_PawnOfferYes(Pawn pawn)
        {
            OfferPawn = pawn;

            if(!OfferPawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(OfferPawn);
            }
        }

        public override void DoAction(EmailMessage emailMessage, EmailBox box, Pawn pawn)
        {
            Map map = Find.AnyPlayerHomeMap;
            if (map == null)
                return;

            if (!CellFinder.TryFindRandomEdgeCellWith(v => !v.Roofed(map) && v.Walkable(map) && !v.Fogged(map) && v.Standable(map), map, 0f, out IntVec3 result))
                return;

            GenSpawn.Spawn(OfferPawn, result, map);
            OfferPawn.SetFaction(Faction.OfPlayer);

            Find.LetterStack.ReceiveLetter("CommOption_PawnOfferYesTitle".Translate(), "CommOption_PawnOfferYes".Translate(OfferPawn.Name.ToStringFull, pawn.Faction.Name), LetterDefOf.PositiveEvent);

            pawn.Faction.TryAffectGoodwillWith(box.Owner, 20);

            box.Messages.Remove(emailMessage);
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref OfferPawn, "OfferPawn");
        }
    }
}
