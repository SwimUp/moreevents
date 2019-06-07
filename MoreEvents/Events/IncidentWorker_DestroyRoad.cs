using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using RimWorld.Planet;

namespace MoreEvents.Events
{
    public class IncidentWorker_DestroyRoad : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["DestroyRoad"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (!HasRoads())
                return false;

            if (Find.FactionManager.AllFactionsListForReading.Count < 2)
                return false;

            return true;
        }

        private bool HasRoads()
        {
            foreach(var tile in Find.WorldGrid.tiles)
            {
                if (tile.potentialRoads != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<Tile> roadsTiles = new List<Tile>();
            foreach(var t in Find.WorldGrid.tiles)
            {
                if (t.potentialRoads != null)
                    roadsTiles.Add(t);
            }

            if (roadsTiles.Count == 0)
                return false;

            Tile tile = roadsTiles.RandomElement();
            tile.potentialRoads = null;

            SendStandardLetter();

            Find.World.renderer.SetDirty<WorldLayer_Roads>();

            return true;
        }
    }
}
