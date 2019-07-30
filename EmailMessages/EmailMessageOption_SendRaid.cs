using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public class EmailMessageOption_SendRaid : EmailMessageOption
    {
        public override string Label => "SendRaid".Translate();

        public float ThreatPointsMultiplier;
        public int TicksToSend;

        public override void DoAction(EmailMessage message, EmailBox box, Pawn speaker)
        {
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.target = Find.AnyPlayerHomeMap;
            incidentParms.faction = message.Faction;
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(incidentParms.target) * ThreatPointsMultiplier;
            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + TicksToSend, incidentParms);

            Find.LetterStack.ReceiveLetter("EmailMessageOption_SendRaidTitle".Translate(), "EmailMessageOption_SendRaid".Translate(), LetterDefOf.ThreatBig);

            box.DeleteMessage(message);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ThreatPointsMultiplier, "ThreatPointsMultiplier");
            Scribe_Values.Look(ref TicksToSend, "TicksToSend");
        }
    }
}
