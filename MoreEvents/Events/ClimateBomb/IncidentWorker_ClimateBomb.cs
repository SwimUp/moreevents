using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class IncidentWorker_ClimateBomb : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["MechanoidPortal"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            int spawnPoint = GetPlace(Current.Game.AnyPlayerHomeMap);

            ClimateBombSite site = (ClimateBombSite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.ClimateBombSite);
            site.Tile = spawnPoint;
            ClimateBombComp comp = site.GetComponent<ClimateBombComp>();
            comp.SetTimer();
            Find.WorldObjects.Add(site);

            CommunicationDialog dialog = QuestsManager.Communications.AddCommunication(QuestsManager.Communications.UniqueIdManager.GetNextDialogID(), "ClimateBombTitle".Translate(), "ClimateBombDesc".Translate(), incident: def);
            comp.CommunicationDialog = dialog;

            SendStandardLetter(site);

            return true;
        }

        private int GetPlace(Map map)
        {
            int playerTile = map.Tile;
            TileFinder.TryFindPassableTileWithTraversalDistance(playerTile, 9, 20, out int result);

            return result;
        }
    }
}
