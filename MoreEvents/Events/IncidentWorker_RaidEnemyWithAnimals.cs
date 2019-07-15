using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events
{
    public class IncidentWorker_RaidEnemyWithAnimals : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            ResolveRaidPoints(parms);
            if (!TryResolveRaidFaction(parms))
            {
                return false;
            }
            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            ResolveRaidStrategy(parms, combat);
            ResolveRaidArriveMode(parms);
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }
            parms.points = AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
            if (list.Count == 0)
            {
                Log.Error("Got no pawns spawning raid from parms " + parms);
                return false;
            }

            GenerateAnimals(parms.faction, list, (Map)parms.target, parms.points);

            parms.raidArrivalMode.Worker.Arrive(list, parms);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Points = " + parms.points.ToString("F0"));
            foreach (Pawn item in list)
            {
                string str = (item.equipment == null || item.equipment.Primary == null) ? "unarmed" : item.equipment.Primary.LabelCap;
                stringBuilder.AppendLine(item.KindLabel + " - " + str);
            }
            string letterLabel = GetLetterLabel(parms);
            string letterText = GetLetterText(parms, list);
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref letterLabel, ref letterText, GetRelatedPawnsInfoLetterText(parms), informEvenIfSeenBefore: true);
            List<TargetInfo> list2 = new List<TargetInfo>();
            if (parms.pawnGroups != null)
            {
                List<List<Pawn>> list3 = IncidentParmsUtility.SplitIntoGroups(list, parms.pawnGroups);
                List<Pawn> list4 = list3.MaxBy((List<Pawn> x) => x.Count);
                if (list4.Any())
                {
                    list2.Add(list4[0]);
                }
                for (int i = 0; i < list3.Count; i++)
                {
                    if (list3[i] != list4 && list3[i].Any())
                    {
                        list2.Add(list3[i][0]);
                    }
                }
            }
            else if (list.Any())
            {
                list2.Add(list[0]);
            }
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, GetLetterDef(), list2, parms.faction, stringBuilder.ToString());
            MakeLords(parms, list);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
            if (!PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.ShieldBelts))
            {
                for (int j = 0; j < list.Count; j++)
                {
                    Pawn pawn = list[j];
                    if (pawn.RaceProps.Humanlike && pawn.apparel.WornApparel.Any((Apparel ap) => ap is ShieldBelt))
                    {
                        LessonAutoActivator.TeachOpportunity(ConceptDefOf.ShieldBelts, OpportunityType.Critical);
                        break;
                    }
                }
            }

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            return true;
        }

        protected LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            IntVec3 originCell = (!parms.spawnCenter.IsValid) ? pawns[0].PositionHeld : parms.spawnCenter;
            if (parms.faction.HostileTo(Faction.OfPlayer))
            {
                return new LordJob_AssaultColony(parms.faction);
            }
            RCellFinder.TryFindRandomSpotJustOutsideColony(originCell, map, out IntVec3 result);
            return new LordJob_AssistColony(parms.faction, result);
        }

        public virtual void MakeLords(IncidentParms parms, List<Pawn> pawns)
        {
            Map map = (Map)parms.target;
            List<List<Pawn>> list = IncidentParmsUtility.SplitIntoGroups(pawns, parms.pawnGroups);
            int @int = Rand.Int;
            for (int i = 0; i < list.Count; i++)
            {
                List<Pawn> list2 = list[i];
                Lord lord = LordMaker.MakeNewLord(parms.faction, MakeLordJob(parms, map, list2, @int), map, list2);
                lord.numPawnsLostViolently = int.MaxValue;

                if (DebugViewSettings.drawStealDebug && parms.faction.HostileTo(Faction.OfPlayer))
                {
                    Log.Message("Market value threshold to start stealing (raiders=" + lord.ownedPawns.Count + "): " + StealAIUtility.StartStealingMarketValueThreshold(lord) + " (colony wealth=" + map.wealthWatcher.WealthTotal + ")");
                }
            }
        }

        private void GenerateAnimals(Faction faction, List<Pawn> pawns, Map map, float points)
        {
            int numOfAnimals = (int)(pawns.Count * 0.7f);
            float min = Mathf.Clamp(points * 0.35f, 100, 800);
            FloatRange pointsRange = new FloatRange(min, Mathf.Clamp(points * 0.6f, min + 100, 2000));
            for(int i = 0; i < numOfAnimals; i++)
            {
                if (ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(pointsRange.RandomInRange, map.Tile, out PawnKindDef animalKind))
                {
                    PawnGenerationRequest request = new PawnGenerationRequest(animalKind, faction, PawnGenerationContext.NonPlayer);
                    Pawn item = PawnGenerator.GeneratePawn(request);
                    pawns.Add(item);
                }
            }
        }


    }
}
