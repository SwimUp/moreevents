using MoreEvents;
using QuestRim;
using RimWorld;
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

        private int action = 0;

        public EmailOptionWorker_RandomComb1()
        {
            int count = Rand.Range(7, 12);
            var temp = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int val = Rand.Range(0, 2);
                temp.Add(val);
            }

            temp.Shuffle();

            for(int j = 0; j < temp.Count; j++)
            {
                numbers += temp[j];
            }

            action = Rand.Range(0, 4);
        }

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            switch(action)
            {
                case 0:
                    {
                        Utils.SendRaid(Find.FactionManager.OfMechanoids, 1.4f, Rand.Range(2, 5) * 60000);
                        break;
                    }
                case 1:
                    {
                        IncidentDef def = IncidentDefOfLocal.PsychicEmanatorShipPartCrash;
                        def.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(def.category, Find.AnyPlayerHomeMap));
                        break;
                    }
                case 2:
                    {
                        IncidentDef def = IncidentDefOfLocal.Quest_ItemStash;
                        def.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(def.category, Find.World));
                        break;
                    }
                case 3:
                    {
                        IncidentDef def = IncidentDefOfLocal.ResourcePodCrash;
                        def.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(def.category, Find.AnyPlayerHomeMap));
                        break;
                    }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref numbers, "numbers");
            Scribe_Values.Look(ref action, "action");
        }
    }
}
