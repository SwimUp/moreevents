using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul
{
    public static class RaidUtils
    {
        public static void SendRaid(Faction faction, Map map, float points, int arrivalTime, PawnsArrivalModeDef pawnsArrivalModeDef = null, RaidStrategyDef raidStrategyDef = null)
        {
            int @int = Rand.Int;

            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            raidParms.forced = true;
            raidParms.faction = faction;
            raidParms.points = points;
            raidParms.pawnGroupMakerSeed = @int;
            raidParms.generateFightersOnly = true;

            if (raidStrategyDef != null)
                raidParms.raidStrategy = raidStrategyDef;
            else
                ResolveRaidStrategy(raidParms, PawnGroupKindDefOf.Combat);

            if (pawnsArrivalModeDef != null)
                raidParms.raidArrivalMode = pawnsArrivalModeDef;
            else
                ResolveRaidArriveMode(raidParms);

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + arrivalTime, raidParms);
        }

        public static void ResolveRaidArriveMode(IncidentParms parms)
        {
            if (parms.raidArrivalModeForQuickMilitaryAid && !(from d in DefDatabase<PawnsArrivalModeDef>.AllDefs
                                                              where d.forQuickMilitaryAid
                                                              select d).Any((PawnsArrivalModeDef d) => d.Worker.GetSelectionWeight(parms) > 0f))
            {
                parms.raidArrivalMode = ((!(Rand.Value < 0.6f)) ? PawnsArrivalModeDefOf.CenterDrop : PawnsArrivalModeDefOf.EdgeDrop);
            }
            else if (!(from x in parms.raidStrategy.arriveModes
                       where x.Worker.CanUseWith(parms)
                       select x).TryRandomElementByWeight((PawnsArrivalModeDef x) => x.Worker.GetSelectionWeight(parms), out parms.raidArrivalMode))
            {
                Log.Error("Could not resolve arrival mode for raid. Defaulting to EdgeWalkIn. parms=" + parms);
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            }
        }

        public static void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            if (parms.raidStrategy != null)
            {
                return;
            }
            Map map = (Map)parms.target;
            if (!(from d in DefDatabase<RaidStrategyDef>.AllDefs
                  where d.Worker.CanUseWith(parms, groupKind) && (parms.raidArrivalMode != null || (d.arriveModes != null && d.arriveModes.Any((PawnsArrivalModeDef x) => x.Worker.CanUseWith(parms))))
                  select d).TryRandomElementByWeight((RaidStrategyDef d) => d.Worker.SelectionWeight(map, parms.points), out parms.raidStrategy))
            {
                Log.Error("No raid stategy for " + parms.faction + " with points " + parms.points + ", groupKind=" + groupKind + "\nparms=" + parms);
                if (!Prefs.DevMode)
                {
                    parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                }
            }
        }
    }
}
