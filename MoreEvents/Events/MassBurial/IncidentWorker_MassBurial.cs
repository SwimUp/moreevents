using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events.MassBurial
{
    public class IncidentWorker_MassBurial : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MassBurial"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (Find.WorldObjects.CaravansCount == 0)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (Find.WorldObjects.Caravans.TryRandomElement(out Caravan caravan))
            {
                if (TileFinder.TryFindPassableTileWithTraversalDistance(caravan.Tile, 2, 5, out int tile))
                {
                    if (Find.FactionManager.AllFactionsVisible.Where(x => x != Faction.OfPlayer).TryRandomElement(out Faction faction))
                    {
                        MassBurialPlace massBurialPlace = (MassBurialPlace)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.MassBurialPlace);
                        massBurialPlace.Tile = tile;
                        massBurialPlace.SetFaction(faction);

                        Find.WorldObjects.Add(massBurialPlace);

                        SendStandardLetter(massBurialPlace, null, faction.Name);
                    }
                }
            }

            return true;
        }
    }
}
