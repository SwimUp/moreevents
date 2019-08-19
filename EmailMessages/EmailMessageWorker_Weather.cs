using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestRim;
using RimWorld.Planet;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_Weather : EmailMessageWorker
    {
        public override bool PreReceived(EmailMessage message, EmailBox box)
        {
            base.PreReceived(message, box);

            if (Find.WorldObjects.Settlements.Where(x => x.Faction == message.Faction).TryRandomElement(out Settlement set))
            {
                WeatherDef weather = DefDatabase<WeatherDef>.GetRandom();
                string text = string.Format(message.Message, box.Owner.Name, set.Name, weather.LabelCap);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
