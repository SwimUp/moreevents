using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events
{
    public class IncidentWorker_Pocahontas : IncidentWorker_NeutralGroup
    {
        private EventSettings settings => Settings.EventsSettings["Pocahontas"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            return base.CanFireNowSub(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Map map = (Map)parms.target;
            if (!TryResolveParms(parms))
            {
                return false;
            }

            return ArrivalMode(parms);
            //if((int)parms.faction.def.techLevel < (int)TechLevel.Industrial)
            //{
            //    return ArrivalMode(parms);
            //}
            //else
            //{
            //    return ConsoleMode(parms);
            //}
        }

        private bool ConsoleMode(IncidentParms parms)
        {
            return true;
        }

        private bool ArrivalMode(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            List<Pawn> list = SpawnPawns(parms);
            if (list.Count == 0 || list.Count < 1)
            {
                return false;
            }
            RCellFinder.TryFindRandomSpotJustOutsideColony(list[0], out IntVec3 result);
            LordJob_VisitColony lordJob = new LordJob_VisitColony(parms.faction, result);
            LordMaker.MakeNewLord(parms.faction, lordJob, map, list);
            Pawn pawn = list.RandomElement();

            CommunicationDialog dialog = new CommunicationDialog();
            dialog.id = QuestsManager.Communications.UniqueIdManager.GetNextDialogID();
            dialog.CardLabel = "Pocahontas_CardLabel".Translate();
            Pawn offerPawn = list.Where(p => p != pawn).RandomElement();
            CommOption_PawnOfferYes commOption_PawnOfferYes = new CommOption_PawnOfferYes();
            commOption_PawnOfferYes.OfferPawn = offerPawn;
            dialog.Options = new List<CommOption>();
            dialog.Options.Add(commOption_PawnOfferYes);

            Settlement settlement = Find.WorldObjects.Settlements.Where(s => s.Faction == parms.faction).RandomElement();
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"{offerPawn.story.Title}\n{offerPawn.Named("PAWN")}");
            builder.AppendLine(builder.ToString().AdjustedFor(offerPawn));

            Log.Message($"DEF ==> {def.letterText}");
            Log.Message($"SETTLEMENT ==> {settlement.Name}");
            Log.Message($"FACTION --> {parms.faction.Name}");
            Log.Message($"OFFERPAWN --> {offerPawn.Name.ToStringFull}");
            Log.Message($"BUILDER --> {builder.ToString()}");

            string dialogDesc = string.Format(def.letterText, settlement.Name, parms.faction.Name, offerPawn.Name.ToStringFull, offerPawn.Name.ToStringFull, builder.ToString());
            dialog.Description = dialogDesc;

            QuestsManager.Communications.AddQuestPawn(pawn, dialog);

            return true;
        }

        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            if (!(parms.points >= 0f))
            {
                parms.points = Rand.Range(90, 200);
            }
        }
    }
}
