using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimOverhaul.AI
{
    public enum CaravanBattleWinner
    {
        None,
        Player,
        AI
    };

    public class CaravanBattlePlacePlace : MapParent
    {
        public IntVec3 MapSize;

        public CaravanAI CaravanAI => caravanAI;
        private CaravanAI caravanAI;
        private Faction aiFaction;
        private int aiTarget;
        private CaravanArrivalAction aiAction;

        public Caravan PlayerCaravan => playerCaravan;
        private Caravan playerCaravan;

        public CaravanBattleWinner Winner => winner;
        private CaravanBattleWinner winner;

        public CaravanBattlePlacePlace()
        {

        }

        public CaravanBattlePlacePlace(CaravanAI caravanAI, Caravan playerCaravan, IntVec3 mapSize)
        {
            this.caravanAI = caravanAI;
            this.playerCaravan = playerCaravan;
            MapSize = mapSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref aiFaction, "aiFaction");
            Scribe_Deep.Look(ref caravanAI, "caravanAI");
            Scribe_Deep.Look(ref aiAction, "aiAction");
        }

        public void SetPlace(CaravanAI caravanAI, Caravan playerCaravan, IntVec3 mapSize)
        {
            MapSize = mapSize;

            this.caravanAI = caravanAI;
            aiFaction = caravanAI.Faction;
            aiTarget = caravanAI.pather.Destination;
            aiAction = caravanAI.pather.ArrivalAction;

            this.playerCaravan = playerCaravan;
            List<Pawn> aiPawns = new List<Pawn>(caravanAI.pawns);

            caravanAI.Tile = Tile;
            playerCaravan.Tile = Tile;

            Map map = GetOrGenerateMap(Tile, MapSize, null);
            Find.TickManager.Notify_GeneratedPotentiallyHostileMap();

            MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out IntVec3 first, out IntVec3 second);

            CaravanEnterMapUtility.Enter(caravanAI, map, x => CellFinder.RandomSpawnCellForPawnNear(first, map), CaravanDropInventoryMode.DoNotDrop);
            CaravanEnterMapUtility.Enter(playerCaravan, map, x => CellFinder.RandomSpawnCellForPawnNear(second, map), CaravanDropInventoryMode.DoNotDrop);

            LordJob lordJob = new LordJob_AssaultColony(caravanAI.Faction, canKidnap: true, canTimeoutOrFlee: false);
            LordMaker.MakeNewLord(caravanAI.Faction, lordJob, map, aiPawns);

            winner = CaravanBattleWinner.None;

            Find.LetterStack.ReceiveLetter("CaravanBattlePlacePlace_BattleStarted".Translate(), "CaravanBattlePlacePlace_BattleDescription".Translate(playerCaravan.Name, caravanAI.Name), LetterDefOf.ThreatBig);
        }

        public override string GetInspectString()
        {
            return string.Format(def.description, caravanAI.Name);
        }

        public Map GetOrGenerateMap(int tile, IntVec3 mapSize, WorldObjectDef suggestedMapParentDef)
        {
            return GetOrGenerateMapUtility.GetOrGenerateMap(tile, mapSize, suggestedMapParentDef);
        }

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!Map.mapPawns.AnyPawnBlockingMapRemoval)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        public override void Tick()
        {
            if(this.IsHashIntervalTick(200))
            {
                CheckWinner();

                CheckRemoveMapNow();
            }
        }

        public void CheckWinner()
        {
            if (winner == CaravanBattleWinner.None)
            {
                int playerPawnsCount = Map.mapPawns.ColonistCount;
                List<Pawn> enemyPawns = Map.mapPawns.AllPawns.Where(x => x.Faction == aiFaction).ToList();
                int enemyPawnsCount = enemyPawns.Count;

                if (playerPawnsCount == 0 && enemyPawnsCount > 0)
                {
                    winner = CaravanBattleWinner.AI;
                    Find.LetterStack.ReceiveLetter("CaravanBattlePlacePlace_Battle_WinnerAITitle".Translate(), "CaravanBattlePlacePlace_Battle_WinnerAIDesc".Translate(), LetterDefOf.NegativeEvent);

                    caravanAI.pawns.Clear();
                    enemyPawns.ForEach(p =>
                    {
                        p.DeSpawn();

                        Find.WorldPawns.PassToWorld(p);

                        caravanAI.AddPawn(p, false);
                    });

                    caravanAI.AddQueueAction(aiAction, caravanAI.pather.Destination);

                    Find.WorldObjects.Add(caravanAI);
                }
                else if (playerPawnsCount > 0 && enemyPawnsCount == 0)
                {
                    winner = CaravanBattleWinner.Player;
                    Find.LetterStack.ReceiveLetter("CaravanBattlePlacePlace_Battle_WinnerPlayerTitle".Translate(), "CaravanBattlePlacePlace_Battle_WinnerPlayerDesc".Translate(), LetterDefOf.PositiveEvent);
                }
            }
        }

        public override void Notify_CaravanFormed(Caravan caravan)
        {
            if(caravan.IsPlayerControlled && winner != CaravanBattleWinner.Player)
            {
                CaravanUtility.KillRandomPawns(caravan, Rand.Range(90, 200));
            }

            List<int> neighbors1 = new List<int>();
            List<int> neighbors2 = new List<int>();

            Find.WorldGrid.GetTileNeighbors(caravan.Tile, neighbors1);
            Find.WorldGrid.GetTileNeighbors(neighbors1.Where(x => !Find.World.Impassable(x)).RandomElement(), neighbors2);

            if (neighbors2.Where(x => !Find.World.Impassable(x) && !neighbors1.Contains(x) && x != Tile).TryRandomElement(out int result))
            {
                caravan.Tile = result;
            }
            else
            {
                if (neighbors1.Where(x => !Find.World.Impassable(x)).TryRandomElement(out int result2))
                {
                    caravan.Tile = result2;
                }
            }

            base.Notify_CaravanFormed(caravan);
        }
    }
}
