using MoreEvents;
using MoreEvents.Events;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events.PlaceBattle
{
    public class IncidentWorker_PlaceBattle : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["PlaceBattle"];
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return TryResolveFactions(out Pair<Faction, Faction> pair);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(map.Tile, 3, 5, out int tile))
                return false;

            if (!TryResolveFactions(out Pair<Faction, Faction> factionPair))
                return false;

            PlaceBattle placeBattle = (PlaceBattle)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.PlaceBattle);
            placeBattle.Tile = tile;
            placeBattle.SetFaction(factionPair.First);
            placeBattle.SecondFaction = factionPair.Second;

            Find.WorldObjects.Add(placeBattle);

            SendStandardLetter(placeBattle);

            return true;
        }

        private bool TryResolveFactions(out Pair<Faction, Faction> factionPair)
        {
            factionPair = default;

            List<Pair<Faction, Faction>> factions = new List<Pair<Faction, Faction>>();
            foreach (var faction in Find.FactionManager.AllFactionsVisible)
            {
                if (faction == Faction.OfPlayer)
                    continue;

                foreach (var faction2 in Find.FactionManager.AllFactionsVisible)
                {
                    if (faction2 == Faction.OfPlayer)
                        continue;

                    if (faction.HostileTo(faction2))
                    {
                        factions.Add(new Pair<Faction, Faction>(faction, faction2));
                    }

                }
            }

            if (factions.Count == 0)
                return false;

            return factions.TryRandomElement(out factionPair);
        }

    }
}
