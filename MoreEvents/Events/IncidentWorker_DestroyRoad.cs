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
            if (!settings.Active)
                return false;

            List<int> roadsTiles = new List<int>();
            for(int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                Tile t = Find.WorldGrid[i];
                if (t.potentialRoads != null)
                    roadsTiles.Add(i);
            }

            if (roadsTiles.Count == 0)
                return false;

            int tileID = roadsTiles.RandomElement();
            Tile tile = Find.WorldGrid[tileID];
            tile.potentialRoads = null;

            SendStandardLetter(parms, new LookTargets(tileID));

            Find.World.renderer.SetDirty<WorldLayer_Roads>();

            return true;
        }
    }
}
