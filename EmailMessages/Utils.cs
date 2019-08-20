using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public static class Utils
    {
        public static bool InRange(this IntRange range, int value)
        {
            if(range.max >= value && range.min <= value)
                return true;

            return false;
        }

        public static void SendRaid(Faction faction, float multiplier, int ticksToSend)
        {
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.target = Find.AnyPlayerHomeMap;
            incidentParms.faction = faction;
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(incidentParms.target) * multiplier;
            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + ticksToSend, incidentParms);
        }
    }
}
