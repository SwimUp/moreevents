﻿using MoreEvents.Quests;
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
    public class WorldObject_Competitions : MapParent
    {
        public string ScorePlaceholder => "WorldObject_Competitions_ScorePlaceHolder".Translate();
        public string YouPlaceholder => "WorldObject_Competitions_YouPlaceHolder".Translate();

        class CompetitionTableRecord : IExposable
        {
            public Pawn Pawn;

            public int SkillLevel;

            public Faction Faction;

            public bool Paid;

            public int Score;

            public void ExposeData()
            {
                Scribe_References.Look(ref Pawn, "Pawn");
                Scribe_References.Look(ref Faction, "Faction");
                Scribe_Values.Look(ref Score, "Score");
                Scribe_Values.Look(ref SkillLevel, "SkillLevel");
                Scribe_Values.Look(ref Paid, "Paid");
            }
        }

        private List<CompetitionTableRecord> competitionTableRecords;

        public int TicksToEnd = 0;

        public bool CompStarted = false;

        public bool CompEnd = false;

        public SkillDef CompetitionSkill;

        public int CompetitionSkillLevelRequired;

        public int RewardCount;

        public Pawn PlayerPawn;

        private int day;
        private int nextDayCheck;

        public CommunicationDialog Dialog;
        public override void Tick()
        {
            base.Tick();

            if (CompEnd)
                return;

            TicksToEnd--;

            if (CompStarted)
            {
                nextDayCheck--;
                if (nextDayCheck <= 0)
                {
                    nextDayCheck = 60000;
                    DayPassed();
                }
            }

            if (TicksToEnd <= 0)
            {
                EndCompetitions();
            }
        }

        public bool Paid(Map map)
        {
            CompetitionTableRecord playerRecord = competitionTableRecords?.FirstOrDefault(x => x.Faction == Faction.OfPlayer);

            if (playerRecord == null)
            {
                Messages.Message("Competitions_NeedToStartForPaid".Translate(), MessageTypeDefOf.NeutralEvent);
                return false;
            }

            if (playerRecord.Paid)
            {
                Messages.Message("Competitions_AlreadyPaid".Translate(), MessageTypeDefOf.NeutralEvent);
                return false;
            }

            if (PriceUtils.TakeSilverFromPlayer(600, map))
            {
                playerRecord.Paid = true;

                Find.LetterStack.ReceiveLetter("Competitions_JudgPaidTitle".Translate(), "Competitions_JudgPaidDesc".Translate(), LetterDefOf.PositiveEvent);

                return true;
            }

            return false;
        }

        public void EndCompetitions()
        {
            CompEnd = true;
            CompStarted = false;

            if (competitionTableRecords != null)
            {
                CompetitionTableRecord winner = competitionTableRecords.FirstOrDefault();
                Find.LetterStack.ReceiveLetter("Competitions_WinnerTitle".Translate(), "Competitions_WinnerDesc".Translate(CompetitionSkill.LabelCap, winner.Faction.Name, winner.Pawn.Name.ToStringFull, winner.Score, RewardCount), LetterDefOf.PositiveEvent);

                if (winner.Faction == Faction.OfPlayer)
                {
                    Thing reward = ThingMaker.MakeThing(ThingDefOf.Silver);
                    reward.stackCount = RewardCount;

                    Map map = Find.AnyPlayerHomeMap;
                    IntVec3 intVec = DropCellFinder.TradeDropSpot(map);
                    DropPodUtility.DropThingsNear(intVec, map, new List<Thing> { reward }, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false);

                    if(winner.Paid && Rand.Chance(0.15f))
                    {
                        Find.LetterStack.ReceiveLetter("Competitions_JudgePaidFailTitle".Translate(), "Competitions_JudgePaidFailDesc".Translate(), LetterDefOf.NegativeEvent);
                        competitionTableRecords.ForEach(x =>
                        {
                            if (x.Faction != Faction.OfPlayer)
                            {
                                x.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -15, reason: "Competitions_JudgePaidReason".Translate());
                            }
                        });
                    }

                    var playerAlliance = QuestsManager.Communications.FactionManager.PlayerAlliance;
                    if(playerAlliance != null)
                    {
                        playerAlliance.GiveTrustToAllFactions(5);
                    }
                }

                foreach (var comp in competitionTableRecords)
                    Find.WorldPawns.RemovePawn(comp.Pawn);

                if(PlayerPawn != null)
                {
                    PlayerPawn.skills.GetSkill(CompetitionSkill).Learn(35000);

                    Find.WorldPawns.PassToWorld(PlayerPawn);
                    PlayerPawn.SetFaction(Faction.OfPlayer);

                    CaravanMaker.MakeCaravan(new List<Pawn> { PlayerPawn }, RimWorld.Faction.OfPlayer, Tile, false);
                }
            }
            else
            {
                Find.LetterStack.ReceiveLetter("Competitions_JustEndTitle".Translate(), "Competitions_JustEndDesc".Translate(CompetitionSkill.LabelCap), LetterDefOf.PositiveEvent);
            }

            if (Dialog != null)
                QuestsManager.Communications.RemoveCommunication(Dialog);

            Find.WorldObjects.Remove(this);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            if (!CompStarted)
            {
                CaravanArrivalAction_StartCompetitions caravanArrivalAction = new CaravanArrivalAction_StartCompetitions(this);
                foreach(var p in CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanArrivalAction.CanVisit(caravan), () => caravanArrivalAction, "EnterToCompetitions".Translate(), caravan, Tile, this))
                {
                    yield return p;
                }
            }

            yield break;
        }

        public void DayPassed()
        {
            if (competitionTableRecords != null)
            {
                foreach (var competition in competitionTableRecords)
                {
                    int factor = Rand.Range(10, 20) + competition.SkillLevel;
                    if (Rand.Chance(0.12f))
                        factor += Rand.Range(4, 8);
                    if (competition.Paid)
                        factor += 7;

                    competition.Score += factor;
                }

                competitionTableRecords.ForEach(x =>
                {
                    if(x.Pawn == null)
                    {
                        Pawn p = PawnGenerator.GeneratePawn(x.Faction.RandomPawnKind());
                        Find.WorldPawns.PassToWorld(p);
                    }
                });

                competitionTableRecords.SortByDescending(x => x.Score);
                CompetitionTableRecord dayLeader = competitionTableRecords.FirstOrDefault();

                StringBuilder builder = new StringBuilder();
                string dayTitle = "Competitions_DayResultTitle".Translate(day, dayLeader.Pawn.Name.ToStringFull, dayLeader.Faction.Name);
                builder.AppendLine(dayTitle);
                for (int i = 0; i < competitionTableRecords.Count; i++)
                {
                    CompetitionTableRecord competitionTableRecord = competitionTableRecords[i];

                    if (competitionTableRecord.Faction == Faction.OfPlayer)
                        builder.Append($"{i + 1}. {competitionTableRecord.Pawn.Name.ToStringFull} - {competitionTableRecord.Score} {ScorePlaceholder} [{YouPlaceholder}]\n");
                    else
                        builder.Append($"{i + 1}. {competitionTableRecord.Pawn.Name.ToStringFull} - {competitionTableRecord.Score} {ScorePlaceholder}\n");
                }

                Find.LetterStack.ReceiveLetter("Competitions_DayResultTitle2".Translate(day), builder.ToString(), LetterDefOf.PositiveEvent);

                day++;
            }
        }

        public void StartCompetitions()
        {
            competitionTableRecords = new List<CompetitionTableRecord>();
            foreach (var faction in Find.FactionManager.AllFactionsVisible)
            {
                if (faction == Faction.OfPlayer)
                    continue;

                Pawn p = PawnGenerator.GeneratePawn(faction.RandomPawnKind());

                Find.WorldPawns.PassToWorld(p);

                competitionTableRecords.Add(new CompetitionTableRecord
                {
                    Pawn = p,
                    Faction = faction,
                    Score = 0,
                    SkillLevel = Rand.Range(8, 20),
                    Paid = Rand.Chance(0.25f) ? true : false
                });
            }

            competitionTableRecords.Add(new CompetitionTableRecord
            {
                Pawn = PlayerPawn,
                Faction = Faction.OfPlayer,
                Score = 0,
                SkillLevel = PlayerPawn.skills.GetSkill(CompetitionSkill).Level
            });

            TicksToEnd = 3 * 62000;
            nextDayCheck = 60000;
            day = 1;

            CompEnd = false;
            CompStarted = true;

            Find.LetterStack.ReceiveLetter("Competitions_EnterToCompetitionsTitle".Translate(), "Competitions_EnterToCompetitionsDesc".Translate(), LetterDefOf.PositiveEvent);
        }

        public override string GetDescription()
        {
            return string.Format(base.GetDescription(), CompetitionSkill.LabelCap, CompetitionSkillLevelRequired);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref TicksToEnd, "TicksToEnd");
            Scribe_Values.Look(ref CompStarted, "CompStarted");
            Scribe_Defs.Look(ref CompetitionSkill, "CompetitionSkill");
            Scribe_Values.Look(ref CompetitionSkillLevelRequired, "CompetitionSkillLevelRequired");
            Scribe_Values.Look(ref RewardCount, "RewardCount");
            Scribe_References.Look(ref PlayerPawn, "PlayerPawn");
            Scribe_Values.Look(ref day, "day");
            Scribe_Collections.Look(ref competitionTableRecords, "competitionTableRecords", LookMode.Deep);
            Scribe_Values.Look(ref nextDayCheck, "nextDayCheck");
            Scribe_References.Look(ref Dialog, "Dialog");
        }
    }
}
