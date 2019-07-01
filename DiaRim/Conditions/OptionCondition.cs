using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim.Conditions
{
    public abstract class OptionCondition
    {
        public int ConditionId = 1;

        [MustTranslate]
        public string DisableReason;

        [Unsaved]
        [TranslationHandle]
        public string untranslatedId;

        public abstract bool CanUse(Pawn p, DialogOption option = null);
    }
}