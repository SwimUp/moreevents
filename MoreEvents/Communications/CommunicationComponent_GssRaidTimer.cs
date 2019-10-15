using QuestRim;
using RimOverhaul.Gss;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DarkNET.CommunicationComps
{
    public class CommunicationComponent_GssRaidTimer : CommunicationComponent_Timer
    {
        public List<Pawn> Pawns;

        public Map Map;

        public CommunicationComponent_GssRaidTimer() : base()
        {

        }

        public CommunicationComponent_GssRaidTimer(int ticks, List<Pawn> pawns, Map map) : base(ticks)
        {
            Pawns = pawns;
            Map = map;
        }

        public override void TimerEnd()
        {
            if (Pawns == null || Map == null)
                return;

            IncidentParms parms = new IncidentParms
            {
                target = Map,
                faction = GssRaids.GssFaction
            };

            PawnsArrivalModeDef pawnsArrivalMode = GssRaids.ResolveRaidArriveMode(parms);
            pawnsArrivalMode.Worker.Arrive(Pawns, parms);

            RaidStrategyDef raidStrategy = GssRaids.ResolveRaidStrategy();
            raidStrategy.Worker.MakeLords(parms, Pawns);

            Find.LetterStack.ReceiveLetter("DarkNet_RaidArriveTitle".Translate(), "DarkNet_RaidArriveDesc".Translate(GssRaids.GssFaction.Name), LetterDefOf.ThreatBig, new LookTargets(parms.spawnCenter, Map));
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref Pawns, "Pawns", LookMode.Reference);
            Scribe_References.Look(ref Map, "Map");
        }
    }
}
