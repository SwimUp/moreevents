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

            if ((int)parms.faction.def.techLevel < (int)TechLevel.Industrial)
            {
                return ArrivalMode(parms);
            }
            else
            {
                return ConsoleMode(parms);
            }
        }

        private bool ConsoleMode(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            PawnKindDef spaceRefugee = PawnKindDefOf.SpaceRefugee;
            Pawn pawn = PawnGenerator.GeneratePawn(spaceRefugee);

            Pawn offerPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.SpaceRefugee);

            int timeOut = 2 * 60000;

            Settlement settlement = Find.WorldObjects.Settlements.Where(s => s.Faction == parms.faction).RandomElement();
            string text = "PawnInfo".Translate().Formatted(offerPawn.Named("PAWN")).AdjustedFor(offerPawn);
            string dialogDesc = string.Format(def.letterText, settlement.Name, parms.faction.Name, offerPawn.Name.ToStringFull, text, "TimerInfo".Translate(timeOut.TicksToDays().ToString("f2")));

            EmailMessage message = QuestsManager.Communications.PlayerBox.FormMessageFrom(parms.faction,
                dialogDesc, def.letterLabel);
            message.Answers = new List<EmailMessageOption>();
            EmailOption_PawnOfferYes emailOption_PawnOfferYes = new EmailOption_PawnOfferYes(offerPawn);

            Letter letter = LetterMaker.MakeLetter("CommunicationDialog_PocahontasTitle".Translate(), "CommunicationDialog_Pocahontas".Translate(parms.faction.Name), LetterDefOf.NegativeEvent);

            TimeComp timeComp = new TimeComp(message.Subject, Faction.OfPlayer, timeOut);
            timeComp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();
            timeComp.Action = new CommOption_ChangeGoodWill(-20);
            timeComp.EndLetter = letter;
            timeComp.Speaker = map.mapPawns.FreeColonists.RandomElement();
            timeComp.Defendant = parms.faction.leader;

            EmailOption_ChangeGoodWill emailOption_ChangeGoodWill = new EmailOption_ChangeGoodWill(-20);
            EmailOption_DeclineDialog emailOption_DeclineDialog = new EmailOption_DeclineDialog(emailOption_ChangeGoodWill);

            message.Answers.Add(emailOption_PawnOfferYes);
            message.Answers.Add(emailOption_DeclineDialog);

            QuestsManager.Communications.PlayerBox.SendMessage(message);

            QuestsManager.Communications.RegisterComponent(timeComp);

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

            CommunicationDialog_Pocahontas dialog = new CommunicationDialog_Pocahontas();
            dialog.id = QuestsManager.Communications.UniqueIdManager.GetNextDialogID();
            dialog.CardLabel = "Pocahontas_CardLabel".Translate();
            dialog.Faction = parms.faction;
            Pawn offerPawn = list.Where(p => p != pawn).RandomElement();
            CommOption_PawnOfferYes commOption_PawnOfferYes = new CommOption_PawnOfferYes();
            commOption_PawnOfferYes.OfferPawn = offerPawn;
            CommOption_DeclineDialog commOption_DeclineDialog = new CommOption_DeclineDialog();
            commOption_DeclineDialog.ExecuteOption = new CommOption_ChangeGoodWill(-35);
            dialog.Options = new List<CommOption>();
            dialog.Options.Add(commOption_PawnOfferYes);
            dialog.Options.Add(commOption_DeclineDialog);

            Settlement settlement = Find.WorldObjects.Settlements.Where(s => s.Faction == parms.faction).RandomElement();
            string text = "PawnInfo".Translate().Formatted(offerPawn.Named("PAWN")).AdjustedFor(offerPawn);
            string dialogDesc = string.Format(def.letterText, settlement.Name, parms.faction.Name, offerPawn.Name.ToStringFull, text, "ActualOnlyThisMap".Translate());
            dialog.Description = dialogDesc;

            dialog.ShowInConsole = false;
            QuestsManager.Communications.AddCommunication(dialog);
            QuestsManager.Communications.AddQuestPawn(pawn, dialog);

            Find.LetterStack.ReceiveLetter(def.letterLabel, dialogDesc, def.letterDef);

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
