using MoreEvents;
using MoreEvents.Events;
using MoreEvents.Quests;
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
    public class Quest_PrisonShipAccident : QuestRim.Quest
    {
        public override QuestDef RelatedQuestDef => QuestDefOfLocal.Quest_None;

        public override string CardLabel => IncidentDef.LabelCap;

        public override string Description => IncidentDef.letterText;

        public override string ExpandingIconPath => Visited ? "Map/Quest_PrisonShipAccident_Send" : "Map/Quest_PrisonShipAccident_Pris";

        public override string PlaceLabel => Visited ? "Quest_PrisonShipAccident_PlaceLabel2".Translate() : "Quest_PrisonShipAccident_PlaceLabel".Translate();

        public List<Pawn> DropedPawns = new List<Pawn>();

        public override bool HasExitCells => false;

        public override bool UseLeaveCommand => true;

        public bool Visited = false;

        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            Site.RemoveAfterLeave = false;

            GenerateShipParts(map);

            GeneratePrisoners(map);
        }

        public override void PostMapRemove(Map map)
        {
            base.PostMapRemove(map);

            if (TileFinder.TryFindPassableTileWithTraversalDistance(Site.Tile, 3, 5, out int tile))
            {
                Site.Tile = tile;
                Visited = true;

                ResetIcon();
            }
        }

        public override string GetInspectString()
        {
            return "InspectString_Timer".Translate(TicksToPass.TicksToDays().ToString("f2"));
        }

        public void CheckPrisoners()
        {
            if (DropedPawns == null)
                return;

            for(int i = DropedPawns.Count - 1; i >= 0; i--)
            {
                Pawn p = DropedPawns[i];

                if (p == null || p.Dead)
                    DropedPawns.RemoveAt(i);
            }
        }

        public override void Notify_CaravanFormed(QuestSite site, Caravan caravan)
        {
            base.Notify_CaravanFormed(site, caravan);

            if (!caravan.pawns.InnerListForReading.Any(x => DropedPawns.Contains(x)))
            {
                Site.EndQuest(null, EndCondition.None);

                Find.LetterStack.ReceiveLetter("Quest_PrisonShipAccident_NCTitle".Translate(), "Quest_PrisonShipAccident_NCDesc".Translate(), LetterDefOf.NeutralEvent);
            }

            DropedPawns.RemoveAll(x => !caravan.ContainsPawn(x));

            UnlimitedTime = false;
            TicksToPass = 4 * 60000;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            if (!Visited)
            {
                CaravanArrivalAction_EnterToQuestMapBasic caravanArrivalAction_EnterToQuestMapBasic = new CaravanArrivalAction_EnterToQuestMapBasic(mapParent);
                return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => caravanArrivalAction_EnterToQuestMapBasic, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
            }
            else
            {
                CaravanArrivalAction_TransferPrisoners caravanArrivalAction_TransferPrisoners = new CaravanArrivalAction_TransferPrisoners(mapParent, this);
                return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanArrivalAction_TransferPrisoners.CanVisit(caravan), () => caravanArrivalAction_TransferPrisoners, "TransferPrisoners".Translate(), caravan, mapParent.Tile, mapParent);
            }
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
                Faction faction = Faction.OfAncientsHostile;

                Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, faction);

                Find.WorldPawns.PassToWorld(pawn);

                DropedPawns.Add(pawn);

                IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(map.Center, map, 99999);

                GenSpawn.Spawn(pawn, spawnPos, map);

                LordJob_DefendBase lordJob_DefendBase = new LordJob_DefendBase(faction, spawnPos);
                Lord lord = LordMaker.MakeNewLord(faction, lordJob_DefendBase, map);

                lord.AddPawn(pawn);
            }
        }

        private void GiveTraits(Pawn p)
        {
            p.story.traits.allTraits.Clear();

            p.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath));

            if(Rand.Chance(0.35f))
                p.story.traits.GainTrait(new Trait(TraitDefOf.Cannibal));

            if (Rand.Chance(0.25f))
                p.story.traits.GainTrait(new Trait(TraitDefOf.Pyromaniac));
        }
        public override void GenerateRewards()
        {
            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = Rand.Range(300, 700);

            Rewards = new List<Thing>
            {
                silver
            };
        }

        public override void EndQuest(Caravan caravan = null, EndCondition condition = EndCondition.None)
        {
            QuestsManager.Communications.RemoveQuest(this, condition);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref DropedPawns, "DropedPawns", LookMode.Reference);
            Scribe_Values.Look(ref Visited, "Visited");
        }
    }
}
