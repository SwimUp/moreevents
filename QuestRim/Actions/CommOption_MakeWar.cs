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
            var alliance = interaction.Alliance;
            if (alliance != null && alliance.FactionOwner == speaker.Faction)
            {
                Messages.Message("CommOption_MakeWar_InAlliance".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            Find.WindowStack.Add(new MakeWarWindow(interaction, speaker, defendant));
        }

    }
}
