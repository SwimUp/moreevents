using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents;
using RimWorld;

namespace EmailMessages
{
    public class EmailMessageWorker_SpaceBattle : EmailMessageWorker_CheckIncident
    {
        public override IncidentDef IncidentDef => IncidentDefOfLocal.SpaceBattle;
    }
}
