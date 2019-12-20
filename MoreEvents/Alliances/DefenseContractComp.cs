using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Alliances
{
    public class DefenseContractComp : AllianceAgreementComp
    {
        private FightersLevel fightersLevel;

        public PawnsArrivalModeDef PawnsArrivalModeDef => PawnsArrivalModeDefOf.CenterDrop;

        private bool alreadyHelped = false;
        private int leaveTicks;

        private List<Pawn> pawnsData = new List<Pawn>();

        public DefenseContractComp()
        {

        }

        public DefenseContractComp(Alliance alliance, FactionInteraction signer, FactionInteraction owner, FightersLevel fightersLevel, int daysDuration)
        {
            AllianceAgreementDef = AllianceAgreementDefOfLocal.DefenseContractAgreement;
            Alliance = alliance;

            SignedFaction = signer;
            OwnerFaction = owner;
            SignTicks = Find.TickManager.TicksGame;
            EndTicks = Find.TickManager.TicksGame + (daysDuration * 60000);
            this.fightersLevel = fightersLevel;
        }

        public override void Tick()
        {
            base.Tick();

            if(alreadyHelped)
            {
                if(leaveTicks < Find.TickManager.TicksGame)
                {
                    alreadyHelped = false;
                    HelpEnd();
                }
            }
        }

        public void HelpEnd()
        {
            foreach(var pawnData in pawnsData)
            {
                if(pawnData != null && !pawnData.Dead)
                {
                    pawnData.SetFaction(SignedFaction.Faction);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref fightersLevel, "fightersLevel");
            Scribe_Values.Look(ref alreadyHelped, "alreadyHelped");
            Scribe_Values.Look(ref leaveTicks, "leaveTicks");

            Scribe_Collections.Look(ref pawnsData, "pawnsData", LookMode.Reference);
        }

        public override void End(AgreementEndReason agreementEndReason)
        {
            Find.LetterStack.ReceiveLetter("DefenseContractComp_EndTitle".Translate(), "DefenseContractComp_EndDesc".Translate(SignedFaction.Faction.Name), LetterDefOf.NeutralEvent);

            base.End(agreementEndReason);
        }

        public void SendHelp(IncidentParms parms)
        {
            if (alreadyHelped)
                return;

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = SignedFaction.Faction,
                groupKind = PawnGroupKindDefOf.Combat,
                points = (int)fightersLevel
            };

            List<Pawn> pawns = new List<Pawn>();
            int count = Rand.Range(3, 5);
            foreach(var p in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms))
            {
                if (pawns.Count == count)
                    break;

                pawns.Add(p);
            }

            PawnsArrivalModeDef pawnsArrivalMode = PawnsArrivalModeDef;
            pawnsArrivalMode.Worker.Arrive(pawns, parms);

            foreach(var pawn in pawns)
            {
                pawn.SetFaction(OwnerFaction.Faction);

                pawnsData.Add(pawn);
            }

            alreadyHelped = true;
            leaveTicks = Find.TickManager.TicksGame + 60000;

            Find.LetterStack.ReceiveLetter("DefenseContractComp_HelpTitle".Translate(), "DefenseContractComp_HelpDesc".Translate(SignedFaction.Faction.Name), LetterDefOf.PositiveEvent);
        }
    }
}
