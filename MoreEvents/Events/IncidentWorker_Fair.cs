using MoreEvents;
using MoreEvents.Events;
using QuestRim;
using RimOverhaul.Events.Fair;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Events
{
    public class IncidentWorker_Fair : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Fair"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TileFinder.TryFindPassableTileWithTraversalDistance(map.Tile, 10, 15, out int newTile))
                return false;

            if (!Find.FactionManager.AllFactionsVisible.Any(x => {
                if (x == Faction.OfPlayer)
                    return false;

                var relation = x.RelationKindWith(Faction.OfPlayer);

                if (relation == FactionRelationKind.Ally || relation == FactionRelationKind.Neutral)
                    return true;

                return false;

            }))
                return false;

            var dialog = QuestsManager.Communications.MakeDialogFromIncident(def);

            FairPlace fairPlace = (FairPlace)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.FairPlace);
            fairPlace.Tile = newTile;
            fairPlace.SetFaction(Faction.OfPlayer);
            fairPlace.SetTimer(10);
            fairPlace.CommunicationDialog = dialog;
            DefDatabase<MapGeneratorBlueprints.MapGenerator.MapGeneratorDef>.AllDefsListForReading.Where(gen => gen.targetTags != null && gen.targetTags.Contains(fairPlace.MapGeneratorTag)).TryRandomElementByWeight(w => w.Commonality, out fairPlace.MapGenerator);

            QuestsManager.Communications.AddCommunication(dialog);

            Find.WorldObjects.Add(fairPlace);

            SendStandardLetter(fairPlace);

            return true;
        }
    }
}
