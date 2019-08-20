using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_CheckIncident : EmailMessageWorker
    {
        public virtual IncidentDef IncidentDef { get; }

        public override bool CanReceiveNow()
        {
            StoryState state = Find.World.StoryState;
            if (state.lastFireTicks.TryGetValue(IncidentDef, out int value))
            {
                var val = (float)(Find.TickManager.TicksGame - value) / 60000f;
                if(val <= 7)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
