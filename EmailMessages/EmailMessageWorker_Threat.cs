using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestRim;
using RimWorld;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_Threat : EmailMessageWorker
    {
        public override bool PreReceived(EmailMessage message, EmailBox box)
        {
            Map map = Find.AnyPlayerHomeMap;
            Pawn first = map.mapPawns.FreeColonists.RandomElement();
            Pawn second = null;
            if (!map.mapPawns.FreeColonists.Where(x => x != first).TryRandomElement(out second))
                return false;

            message.Message = string.Format(message.Message, first.Name.ToStringFull, second.Name.ToStringFull);

            first.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.FearFromMessage, first);
            second.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfLocal.FearFromMessage, second);

            return base.PreReceived(message, box);
        }
    }
}
