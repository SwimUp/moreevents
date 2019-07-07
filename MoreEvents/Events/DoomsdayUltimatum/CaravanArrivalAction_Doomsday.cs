using MapGeneratorBlueprints.MapGenerator;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class CaravanArrivalAction_Doomsday : CaravanArrivalAction_EnterToMap
    {
        public override IntVec3 MapSize => new IntVec3(250, 1, 250);

        public CaravanArrivalAction_Doomsday()
        {

        }

        public CaravanArrivalAction_Doomsday(MapParent mapParent) : base(mapParent)
        {
        }
        
        public override FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
        }

        public override Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            if (map == null)
            {
                map = Verse.MapGenerator.GenerateMap(mapSize, MapParent, MapGeneratorDefOfLocal.EmptyMap);
            }
            return map;
        }

        public override void DoEnter(Caravan caravan)
        {
            Pawn t = caravan.PawnsListForReading[0];
            bool flag2 = !MapParent.HasMap;
            Verse.Map orGenerateMap = GetOrGenerateMap(MapParent.Tile, MapSize, null);
            if (flag2)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsSite".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, informEvenIfSeenBefore: true);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterCaravanEnteredMap".Translate(caravan.Label, Site).CapitalizeFirst());
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {Site.Label}", stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
            else
            {
                Find.LetterStack.ReceiveLetter($"{Translator.Translate("CaravanEnteredMassiveFire")} {Site.Label}", "LetterCaravanEnteredMap".Translate(caravan.Label, Site).CapitalizeFirst(), LetterDefOf.NeutralEvent, t);
            }
            Verse.Map map = orGenerateMap;
            IntVec3 enterPos = new IntVec3(82, 0, 5);
            CaravanEnterMapUtility.Enter(caravan, map, (Pawn p) => enterPos);

            DoomsdaySite site = (DoomsdaySite)MapParent;
            var comp = site.GetComponent<DoomsdayUltimatumComp>();
            if (comp.HelpingFactions != null)
            {
                CacheAndChangeRelations(comp);

                GeneratePawns(comp, ref enterPos, map);
            }

            comp.Dialog = null;
            QuestsManager.Communications.RemoveCommunication(comp.Dialog);
        }

        private void GeneratePawns(DoomsdayUltimatumComp comp, ref IntVec3 enterPos, Map map)
        {
            Log.Clear();

            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                points = Rand.Range(600, 1500),
                generateFightersOnly = true,
                groupKind = PawnGroupKindDefOf.Combat
            };

            List<Pawn> pawns = new List<Pawn>();
            foreach (var faction in comp.HelpingFactions)
            {
                pawnGroupMakerParms.faction = faction;
                int pawnsCount = Rand.Range(2, 3);
                int i = 0;
                IEnumerable<Pawn> generatedPawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
                foreach(var p in generatedPawns)
                {
                    if (i == pawnsCount)
                        break;

                    i++;
                    GenSpawn.Spawn(p, enterPos, map);
                    pawns.Add(p);
                }
            }

            LordJob lordJob = new LordJob_AssaultColony(Faction.OfPlayer, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
            Lord lord = LordMaker.MakeNewLord(Faction.OfPlayer, lordJob, map);
            lord.numPawnsLostViolently = int.MaxValue;

            foreach (var p in pawns)
                lord.AddPawn(p);
        }

        private void CacheAndChangeRelations(DoomsdayUltimatumComp comp)
        {
            comp.CachedRelations = new List<FactionRelation>();
            comp.CachedFactions = new List<Faction>();
            foreach (var faction in comp.HelpingFactions)
            {
                foreach (var faction2 in comp.HelpingFactions)
                {
                    if (faction2 == faction)
                        continue;

                    FactionRelation mainRelation = faction.RelationWith(faction2);
                    if (mainRelation != null)
                    {
                        FactionRelation relation = new FactionRelation();
                        relation.other = mainRelation.other;
                        relation.kind = mainRelation.kind;

                        comp.CachedRelations.Add(relation);
                        comp.CachedFactions.Add(faction);

                        faction.TrySetRelationKind(faction2, FactionRelationKind.Ally);
                    }
                }

                FactionRelation withPlayer = faction.RelationWith(Faction.OfPlayer);
                if(withPlayer != null)
                {
                    FactionRelation relation = new FactionRelation();
                    relation.other = withPlayer.other;
                    relation.kind = withPlayer.kind;

                    comp.CachedRelations.Add(relation);
                    comp.CachedFactions.Add(faction);

                    faction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Ally);
                }
            }
        }
    }
}
