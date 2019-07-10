using MoreEvents.AI;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace MoreEvents.Communications
{
    public class AssistCaravanWithFixedInventoryComp : CommunicationComponent
    {
        public List<Thing> ContainedItems;
        private int ticks = 0;
        private Faction faction;
        private Map map;

        public AssistCaravanWithFixedInventoryComp()
        {

        }

        public AssistCaravanWithFixedInventoryComp(List<Thing> items, int ticks, Faction faction, Map map)
        {
            ContainedItems = items;
            this.ticks = ticks;
            this.faction = faction;
            this.map = map;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref ContainedItems, "containedItems", LookMode.Deep);
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_References.Look(ref faction, "faction");
            Scribe_References.Look(ref map, "map");
        }

        public override void Tick()
        {
            base.Tick();

            ticks--;

            if(ticks <= 0)
            {
                SpawnCaravan();
            }
        }

        private void SpawnCaravan()
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = faction,
                groupKind = PawnGroupKindDefOf.Combat,
                points = Rand.Range(200, 500),
            };
            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();

            if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(map) && c.Walkable(map) && c.Standable(map), map, 0f, out IntVec3 pos))
            {
                foreach (var pawn in pawns)
                {
                    GenSpawn.Spawn(pawn, pos, map);
                }
            }

            Pawn p = pawns.RandomElement();
            foreach(var item in ContainedItems)
            {
                Log.Message($"ITEM --> {item}");
                p.inventory.innerContainer.TryAdd(item);
            }

            Find.LetterStack.ReceiveLetter("AssistCaravan_GifterArrivedTitle".Translate(), "AssistCaravan_GifterArrived".Translate(), LetterDefOf.PositiveEvent, new LookTargets(pawns[0]));

            RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0], out IntVec3 result);

            LordJob_CaravanBringItems lordJob = new LordJob_CaravanBringItems(result, p);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map, pawns);

            QuestsManager.Communications.RemoveComponent(this);
        }
    }
}
