using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class CommOption_NonAgressionPact : InteractionOption
    {
        public override string Label => "CommOption_NonAgressionPact_Label".Translate(SilverCost);

        public bool Signed = false;

        public override Color TextColor => Signed ? Color.yellow : Color.white;

        public int SilverCost => 1500;

        public override void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant)
        {
            if (Signed)
                return;

            Map map = speaker.Map;
            int playerSilver = map.resourceCounter.Silver;
            if (playerSilver >= SilverCost)
            {
                int remaining = SilverCost;
                List<Thing> silver = map.listerThings.ThingsOfDef(ThingDefOf.Silver);
                for(int i = 0; i < silver.Count; i++)
                {
                    Thing item = silver[i];

                    int num = Mathf.Min(remaining, item.stackCount);
                    item.SplitOff(num).Destroy();
                    remaining -= num;
                }

                Success(interaction.Faction, speaker);
            }
            else
            {
                Messages.Message("CommOption_NonAgressionPact_NotEnoughSilver".Translate(SilverCost, playerSilver), MessageTypeDefOf.NeutralEvent, false);
            }
        }

        private void Success(Faction faction, Pawn speaker)
        {
            Signed = true;
            SkillRecord skillRecord = speaker.skills.GetSkill(SkillDefOf.Social);
            int signDays = Mathf.Max(skillRecord.Level, 10);

            NonAfressionPackComp comp = new NonAfressionPackComp(signDays, faction);
            comp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();
            QuestsManager.Communications.RegisterComponent(comp);

            Find.LetterStack.ReceiveLetter("CommOption_NonAgressionPact_SuccessTitle".Translate(), "CommOption_NonAgressionPact_Success".Translate(faction.Name, speaker.Name.ToStringFull, signDays), LetterDefOf.PositiveEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Signed, "Signed");
        }
    }
}
