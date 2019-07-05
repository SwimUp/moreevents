using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Actions
{
    public class Action_DeteriorationRelations : OptionAction
    {
        public int Amount;

        public override void DoAction(DialogOption option)
        {
            Pawn Speaker = option.Dialog.Speaker;
            Pawn Defendant = option.Dialog.Defendant;

            if (Speaker == null || Defendant == null)
                return;

            Faction faction1 = Speaker.Faction;
            Faction faction2 = Defendant.Faction;

            faction2.TryAffectGoodwillWith(faction1, Amount);
        }
    }
}
