using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestRim;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_NiceJoke : EmailMessageOption
    {
        public override string Label => "NiceJoke".Translate();

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var building in Find.AnyPlayerHomeMap.listerBuildings.allBuildingsColonist)
            {
                if(Rand.Chance(0.15f))
                {
                    builder.Append($"{building.LabelCap}, ");
                }
            }
            builder.Remove(builder.Length - 1, 1);

            EmailMessage msg = box.FormMessageFrom(message.Faction, "NiceJoke_Explain".Translate(builder.ToString()), "NiceJoke_Subject".Translate());
            msg.Answers.Add(new EmailMessageOption_Blef());
            msg.Answers.Add(new EmailMessageOption_DontDoIs());

            box.SendMessage(msg);
        }
    }
}
