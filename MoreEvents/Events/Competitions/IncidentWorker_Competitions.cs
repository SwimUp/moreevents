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

namespace RimOverhaul.Events.Competitions
{
    public class IncidentWorker_Competitions : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["Competitions"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = Find.AnyPlayerHomeMap;

            if (!TileFinder.TryFindPassableTileWithTraversalDistance(map.Tile, 5, 7, out int newTile))
                return false;

            SkillDef compSkill = DefDatabase<SkillDef>.GetRandom();
            int level = Rand.Range(8, 20);

            WorldObject_Competitions place = (WorldObject_Competitions)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.CompetitionsPlace);
            place.Tile = newTile;
            place.SetFaction(Faction.OfPlayer);
            place.CompetitionSkill = compSkill;
            place.CompetitionSkillLevelRequired = level;
            place.RewardCount = level * 140;
            place.TicksToEnd = 5 * 60000;

            Find.WorldObjects.Add(place);

            string label = def.label;
            string description = string.Format(def.letterText, compSkill.LabelCap, place.RewardCount, level);

            Find.LetterStack.ReceiveLetter(label, description, def.letterDef, new LookTargets(place));

            var dialog = QuestsManager.Communications.MakeDialogFromIncident(def, new List<CommOption>
            {

            });
            dialog.Description = description;

            QuestsManager.Communications.AddCommunication(dialog);

            return true;
        }
    }
}
