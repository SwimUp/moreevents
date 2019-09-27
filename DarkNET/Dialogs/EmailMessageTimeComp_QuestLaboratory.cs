using MoreEvents;
using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.Dialogs
{
    public class EmailMessageTimeComp_QuestLaboratory : TimeComp
    {
        public EmailMessageTimeComp_QuestLaboratory() : base()
        {

        }

        public EmailMessageTimeComp_QuestLaboratory(string emailSubject)
        {
            EmailMessageSubject = emailSubject;
            EmailBoxOwner = Faction.OfPlayer;
            TicksToRemove = 10 * 60000;
        }

        public override void RemoveNow()
        {
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.World);
            parms.forced = true;

            var incident = new FiringIncident(IncidentDefOfLocal.HighMutantPopulation, null, parms);
            Find.Storyteller.TryFire(incident);

            base.RemoveNow();
        }
    }
}
