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
using Verse.AI.Group;

namespace RimOverhaul.Quests
{
    public class Quest_PrisonShipAccident : Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => IncidentDef.LabelCap;

        public override string Description => IncidentDef.letterText;

        public override string ExpandingIconPath => DropedPawns.Count > 0 ? "" : "";

        public override string PlaceLabel => DropedPawns.Count > 0 ? "Quest_PrisonShipAccident_PlaceLabel2".Translate() : "Quest_PrisonShipAccident_PlaceLabel".Translate();

        public List<Pawn> DropedPawns = new List<Pawn>();

        public override bool HasExitCells => true;

        public override bool UseLeaveCommand => false;

        public bool Visited = false;

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            if (!Visited)
            {
                Site.RemoveAfterLeave = false;

                GenerateShipParts(map);

                GeneratePrisoners(map);
            }
        }

        public override void Notify_CaravanFormed(QuestSite site, Caravan caravan)
        {
            base.Notify_CaravanFormed(site, caravan);

            Log.Message("YEP");

            foreach (var p in caravan.pawns)
                Log.Message(p.Name.ToStringFull);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            CaravanArrivalAction_EnterToQuestMapBasic caravanArrivalAction_EnterToQuestMapBasic = new CaravanArrivalAction_EnterToQuestMapBasic(mapParent);
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanArrivalAction_EnterToQuestMapBasic, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        private void GenerateShipParts(Map map)
        {
            int shipPartsCount = Rand.Range(8, 20);
            for(int i = 0; i < shipPartsCount; i++)
            {
                GenSpawn.Spawn(ThingDefOf.ShipChunk, CellFinder.RandomClosewalkCellNear(map.Center, map, 99999), map);
            }
        }

        private void GeneratePrisoners(Map map)
        {
            int prisonersCount = Rand.Range(2, 6);
            for(int i = 0; i < prisonersCount; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfAncientsHostile);

                Find.WorldPawns.PassToWorld(pawn);

                DropedPawns.Add(pawn);

                IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(map.Center, map, 99999);

                GenSpawn.Spawn(pawn, spawnPos, map);

                LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(Faction.OfAncientsHostile, spawnPos);
                Lord lord = LordMaker.MakeNewLord(Faction.OfAncientsHostile, lordJob_DefendBase, map);

                lord.AddPawn(pawn);
            }
        }
        public override void GenerateRewards()
        {
            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = Rand.Range(600, 1500);

            Rewards = new List<Thing>
            {
                silver
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref DropedPawns, "DropedPawns", LookMode.Reference);
            Scribe_Values.Look(ref Visited, "Visited");
        }
    }
}
