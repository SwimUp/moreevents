using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class CommOption_SubscribeScout : InteractionOption
    {
        public override string Label => "CommOption_SubscribeScout_Label".Translate();
        public override Color TextColor => Active ? Color.yellow : Color.white;

        public bool Active = false;

        public override void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant)
        {
            bool subAlready = ScoutingComp.ScoutAlready(interaction.Faction, out ScoutingComp outComp);

            StringBuilder builder = new StringBuilder();
            builder.Append("CommOption_SubscribeScout_SubTitle".Translate());

            DiaOption diaOption = new DiaOption("CommOption_SubscribeScout_SubButton".Translate());
            if (subAlready)
            {
                builder.Append("CommOption_SubscribeScout_SubAlready".Translate(outComp.GetDays()));
                diaOption.Disable("CommOption_SubscribeScout_SubAlreadyOption".Translate());
            }
            else if(interaction.Faction.PlayerGoodwill < 80)
            {
                diaOption.Disable("CommOption_SubscribeScout_SubGoodWillOption".Translate());
            }

            diaOption.action = delegate
            {
                if(!subAlready)
                {
                    ScoutingComp.GiveScoutingComp(interaction.Faction, 2, 14, 5);
                }
            };
            diaOption.resolveTree = true;

            DiaOption diaOption2 = new DiaOption("CommOption_ExitNode".Translate());
            diaOption2.resolveTree = true;

            DiaNode diaNode = new DiaNode(builder.ToString());
            diaNode.options.Add(diaOption);
            diaNode.options.Add(diaOption2);

            Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode, delayInteractivity: true);

            Find.WindowStack.Add(dialog_NodeTree);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Active, "Active");
        }
    }
}
