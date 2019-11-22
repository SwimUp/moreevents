using MoreEvents.AI;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.AI
{
    public class CaravanArrivalAction_AIEnterCaravan : CaravanArrivalAction
    {
        public override string Label => mapParent.LabelCap;

        public override string ReportString => mapParent.LabelCap;

        private MapParent mapParent;

        public CaravanArrivalAction_AIEnterCaravan()
        {

        }

        public CaravanArrivalAction_AIEnterCaravan(MapParent mapParent)
        {
            this.mapParent = mapParent;
        }

        public override void Arrived(Caravan caravan)
        {
            Enter(caravan);
        }

        public virtual void Enter(Caravan caravan)
        {
            if (!mapParent.HasMap)
                return;

            Map map = mapParent.Map;

            List<Pawn> pawns = new List<Pawn>(caravan.PawnsListForReading);
            Pawn randomPawn = pawns[0];

            Faction faction = randomPawn.Faction;

            if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(map) && c.Walkable(map) && c.Standable(map), map, 0f, out IntVec3 pos))
            {
                foreach (var pawn in pawns)
                {
                    GenSpawn.Spawn(pawn, pos, map);

                    if (pawn.trader != null)
                        pawn.trader = null;
                }
            }

            Find.WorldObjects.Remove(caravan);

            Find.LetterStack.ReceiveLetter("AssistCaravan_GifterArrivedTitle".Translate(), "AssistCaravan_GifterArrived".Translate(), LetterDefOf.PositiveEvent, new LookTargets(randomPawn));

            RCellFinder.TryFindRandomSpotJustOutsideColony(randomPawn, out IntVec3 result);

            LordJob_CaravanBringItems lordJob = new LordJob_CaravanBringItems(result, pawns);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map, pawns);
        }

        public virtual FloatMenuAcceptanceReport CanVisit(Caravan caravan, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned)
            {
                return false;
            }
            return true;
        }

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            FloatMenuAcceptanceReport floatMenuAcceptanceReport = true;
            if (!(bool)floatMenuAcceptanceReport)
            {
                return floatMenuAcceptanceReport;
            }
            if (mapParent != null && mapParent.Tile != destinationTile)
            {
                return false;
            }
            return CanVisit(caravan, mapParent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mapParent, "mapParent");
        }
    }
}
