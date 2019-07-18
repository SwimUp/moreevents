using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Communications
{
    public class CommunicationDialog_Pocahontas : CommunicationDialog
    {
        public override void Destroy()
        {
            Find.LetterStack.ReceiveLetter("CommunicationDialog_PocahontasTitle".Translate(), "CommunicationDialog_Pocahontas".Translate(Faction.Name), LetterDefOf.NegativeEvent);

            Faction.TryAffectGoodwillWith(Faction.OfPlayer, -20);

            base.Destroy();
        }
    }
}
