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

namespace RimOverhaul.Events.ConcantrationCamp
{
    public class IncidentWorker_ConcantrationCamp : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!TileFinder.TryFindNewSiteTile(out int tile))
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TileFinder.TryFindNewSiteTile(out int tile, 4, 16))
                return false;

            Faction enemyFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.Pirate);

            ConcantrationCamp concantrationCamp = (ConcantrationCamp)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.ConcantrationCamp);
            concantrationCamp.Tile = tile;
            concantrationCamp.SetFaction(enemyFaction);
            concantrationCamp.GeneratePawns(Rand.Range(1, 2));
            concantrationCamp.Timer = Rand.Range(9, 14) * 60000;

            QuestsManager.Communications.AddCommunication(QuestsManager.Communications.UniqueIdManager.GetNextDialogID(), def.LabelCap, def.letterText, enemyFaction, def);

            Find.WorldObjects.Add(concantrationCamp);

            SendStandardLetter(concantrationCamp);

            return true;
        }
    }
}
