using MoreEvents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.Events
{
    public class GameCondition_HighMutantPopulation : GameCondition
    {
        private int raidTicks;

        private IntRange nextRaidTicksRangeDays => new IntRange(2, 6); 

        private IntRange pointsRange => new IntRange(1000, 3000);

        private PawnKindDef[] kinds => new PawnKindDef[]
        {
            PawnKindDefOfLocal.BattleRam,
            PawnKindDefOfLocal.Bombardier,
            PawnKindDefOfLocal.Goliath,
            PawnKindDefOfLocal.Termitnator
        };

        public override void Init()
        {
            base.Init();

            raidTicks = nextRaidTicksRangeDays.RandomInRange * 60000;
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            raidTicks--;
            if(raidTicks <= 0)
            {
                raidTicks = nextRaidTicksRangeDays.RandomInRange * 60000;

                DoGenRaid();
            }
        }

        private void DoGenRaid()
        {
            IncidentParms parms = new IncidentParms();
            Faction faction = Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.Mutants);

            bool sended = false;
            List<Pawn> pawns = new List<Pawn>();

            foreach (var map in AffectedMaps)
            {
                if (!RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter, map, 0f))
                {
                    continue;
                }

                sended = true;

                parms.target = map;

                int pawnsCount = pointsRange.RandomInRange / 400;
                for(int i = 0; i < pawnsCount; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(kinds.RandomElement(), faction);

                    pawns.Add(pawn);
                }

                PawnsArrivalModeDefOf.EdgeWalkIn.Worker.Arrive(pawns, parms);

                LordJob lordJob = new LordJob_AssaultColony(faction, canKidnap: true, canTimeoutOrFlee: false);
                if (lordJob != null)
                {
                    LordMaker.MakeNewLord(faction, lordJob, map, pawns);
                }
            }

            if(sended)
            {
                Find.LetterStack.ReceiveLetter("GameCondition_HighMutantPopulation_AttackTitle".Translate(), "GameCondition_HighMutantPopulation_AttackDesc".Translate(), LetterDefOf.ThreatBig, pawns);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref raidTicks, "lastRaidTicks");
        }
    }
}
