using QuestRim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailOptionWorker_RandomComb1 : EmailMessageOption
    {
        public override string Label => numbers;

        private string numbers;

        public EmailOptionWorker_RandomComb1()
        {
            var temp = new List<int>();
            for(int i = 0; i < Rand.Range(4, 10); i++)
            {
                temp[i] = Rand.Range(0, 1);
            }

            temp.Shuffle();

            for(int j = 0; j < temp.Count; j++)
            {
                numbers += temp[j];
            }
        }

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            Utils.SendRaid(Find.FactionManager.OfMechanoids, 1.4f, Rand.Range(2, 5) * 60000);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref numbers, "numbers");
        }
    }
}
