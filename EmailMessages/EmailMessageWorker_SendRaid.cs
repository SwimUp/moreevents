using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageWorker_SendRaid : EmailMessageWorker
    {
        public int TicksToSend;

        public override void OnReceived(EmailMessage message, EmailBox box)
        {
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.target = Find.AnyPlayerHomeMap;
            incidentParms.faction = message.Faction;
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(incidentParms.target);
            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + TicksToSend, incidentParms);
        }
    }
}
