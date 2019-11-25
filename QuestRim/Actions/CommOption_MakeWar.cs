using DiaRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class CommOption_MakeWar : InteractionOption
    {
        public override string Label => "CommOption_MakeWar_Label".Translate();

        public override void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant)
        {
            Find.WindowStack.Add(new MakeWarWindow(interaction, speaker, defendant));
        }

    }
}
